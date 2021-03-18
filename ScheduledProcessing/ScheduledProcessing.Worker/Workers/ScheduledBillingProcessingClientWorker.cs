﻿using Library.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScheduledProcessing.Worker.Domain.Models;
using ScheduledProcessing.Worker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduledProcessing.Worker.Workers
{
    public class ScheduledBillingProcessingClientWorker : BackgroundService
    {
        private readonly IAmountProcessor _processor;
        private readonly IRpcClient<List<Customer>> _customerClient;
        private readonly IRpcClient<List<Billing>> _billingClient;
        private readonly ILogger<ScheduledBillingProcessingClientWorker> _logger;
        private readonly int _millisecondsScheduledTime;

        public ScheduledBillingProcessingClientWorker(
            IRpcClient<List<Customer>> customerClient,
            IRpcClient<List<Billing>> billingClient,
            IAmountProcessor processor,
            IConfiguration config,
            ILogger<ScheduledBillingProcessingClientWorker> logger)
        {
            _processor = processor;
            _customerClient = customerClient;
            _billingClient = billingClient;
            _logger = logger;
            _millisecondsScheduledTime = config.GetSection("MillisecondsScheduledTime").Get<int>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{DateTime.UtcNow:G} Starting scheduled batch processing ...");
            var comparer = new CustomerCpfComparer();
            var batch = new List<Billing>();
            var batchId = Guid.NewGuid().ToString();
            while (true)
            {
                try
                {
                    List<Customer> customers;
                    (batch, customers) = await FetchBatchAsync(batch, batchId);
                    batch = ProcessBatch(batch, customers, comparer, batchId);
                    batchId = await WaitTillNextBatch(_millisecondsScheduledTime);
                }
                catch (Exception ex)
                {
                    // Todo tratar melhor os erros
                    _logger.LogInformation($"{DateTime.UtcNow:G} BatchId: {batchId}, Exceptions: {string.Join(Environment.NewLine, ex.ExtractMessages())}");
                }
            }
        }

        private async Task<(List<Billing>, List<Customer>)> FetchBatchAsync(List<Billing> batch, string batchId)
        {
            var customers = new List<Customer>();
            var billingTask = Task.Run(() =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                batch = _billingClient.CallProcedure(batch);
                stopWatch.Stop();
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Billings ready to process. Elapsed milliseconds {stopWatch.ElapsedMilliseconds}...");
                stopWatch.Reset();
            });
            var customerTask = Task.Run(() =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                customers = _customerClient.CallProcedure("");
                stopWatch.Stop();
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Customers ready to process. Elapsed milliseconds {stopWatch.ElapsedMilliseconds}...");
                stopWatch.Reset();
            });

            await Task.WhenAll(billingTask, customerTask);
            return (batch, customers);
        }

        private List<Billing> ProcessBatch(List<Billing> batch, List<Customer> customers, IComparer<Customer> comparer, string batchId)
        {
            if (customers.Count == 0 || batch.Count == 0)
            {
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Skiping batch. Nothing to process now...");
            }
            else
            {
                var customerForProcessing = new Customer();
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Process started...");
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                Parallel.ForEach(batch, billing =>
                {
                    customerForProcessing.Cpf = billing.Cpf;
                    var index = customers.BinarySearch(customerForProcessing, comparer);
                    customerForProcessing = customers[index];
                    billing = _processor.Process(customerForProcessing, billing);
                    batch.Add(billing);
                });
                stopWatch.Stop();
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Process finished. Elapsed milliseconds {stopWatch.ElapsedMilliseconds}...");
                stopWatch.Reset();
            }

            return batch;
        }

        private async Task<string> WaitTillNextBatch(int millisecondsScheduledTime)
        {
            _logger.LogInformation($"{DateTime.UtcNow:G}  Waiting {millisecondsScheduledTime} milliseconds to process next batch...");
            await Task.Delay(millisecondsScheduledTime);
            return Guid.NewGuid().ToString();
        }
    }
}

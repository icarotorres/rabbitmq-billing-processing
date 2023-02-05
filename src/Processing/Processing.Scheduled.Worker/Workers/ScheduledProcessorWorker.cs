// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Library.Messaging;
using Library.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;

namespace Processing.Scheduled.Worker.Workers
{
    public class ScheduledProcessorWorker : BackgroundService
    {
        private readonly IRpcClient<List<Customer>> _customerClient;
        private readonly IRpcClient<List<Billing>> _billingClient;
        private readonly IAmountProcessor _processor;
        private readonly IComparer<ICpfCarrier> _comparer;
        private readonly ScheduledProcessorSettings _config;
        private readonly ILogger<ScheduledProcessorWorker> _logger;

        public ScheduledProcessorWorker(
            IRpcClient<List<Customer>> customerClient,
            IRpcClient<List<Billing>> billingClient,
            IAmountProcessor processor,
            IComparer<ICpfCarrier> comparer,
            ScheduledProcessorSettings config,
            ILogger<ScheduledProcessorWorker> logger)
        {
            _customerClient = customerClient;
            _billingClient = billingClient;
            _processor = processor;
            _comparer = comparer;
            _config = config;
            _logger = logger;
        }

        // TODO: Extract this logic to another internal method to test it properly
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting scheduled batch processing...");

            var batch = new ProcessBatch();
            var errorCount = 0;
            using var semaphore = new SemaphoreSlim(0, 1);
            do
            {
                if (await semaphore.WaitAsync(_config.MillisecondsScheduledTime))
                {
                    try
                    {
                        batch = await DoExecute(batch);
                        batch.Id = Guid.NewGuid().ToString();
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        _logger.LogError("BatchId: {BatchId}, ErrorCount: {ErrorCount}, Exceptions: {ExceptionMessages}",
                            batch.Id, errorCount, string.Join(Environment.NewLine, ex.ExtractMessages()));
                    }
                }
            } while (_config.MaxErrorCount > errorCount);

            _logger.LogError("exceeded max error count for this instance. ErrorCount: {ErrorCount}", errorCount);
            await StopAsync(stoppingToken);
            await StartAsync(stoppingToken);
        }

        internal async Task<ProcessBatch> DoExecute(ProcessBatch batchProcessed)
        {
            var batchToProcess = await FetchBatchAsync(batchProcessed);
            return ProcessBatch(batchToProcess);
        }

        internal async Task<ProcessBatch> FetchBatchAsync(ProcessBatch batch)
        {
            var billingTask = Task.Run(() =>
            {
                batch.Billings = _billingClient.CallProcedure(batch.Billings);
                _logger.LogInformation("BatchId: {batchId}. Billings ready to process...", batch.Id);
            });
            var customerTask = Task.Run(() =>
            {
                batch.Customers = new List<ICpfCarrier>(_customerClient.CallProcedure(string.Empty));
                _logger.LogInformation("BatchId: {batchId}. Customers ready to process...", batch.Id);
            });

            await Task.WhenAll(billingTask, customerTask);
            return batch;
        }

        internal ProcessBatch ProcessBatch(ProcessBatch batch)
        {
            if (batch.Customers.Count == 0 || batch.Billings.Count == 0)
            {
                _logger.LogInformation("BatchId: {BatchId}. Skiping batch. Nothing to process now...", batch.Id);
                return batch;
            }

            _logger.LogInformation("BatchId: {BatchId}. Process started...", batch.Id);
            batch.Customers.Sort(_comparer);
            Parallel.For(0, batch.Billings.Count, billindIndex =>
            {
                var billing = batch.Billings[billindIndex];
                var customerIndex = batch.Customers.BinarySearch(billing, _comparer);
                if (customerIndex >= 0)
                {
                    var customer = batch.Customers[customerIndex];
                    batch.Billings[billindIndex] = _processor.Process(customer, billing);
                }
            });
            _logger.LogInformation("BatchId: {BatchId}. Process finished...", batch.Id);

            return batch;
        }

        //internal ProcessBatch ProcessBatchJoinGroupSelectMany(ProcessBatch batch)
        //{
        //    batch.Billings = (from b in batch.Billings
        //                      join c in batch.Customers on b.Cpf equals c.Cpf
        //                      group new { customer = c, billing = b } by b.Cpf into g
        //                      select from pair in g select _processor.Process(pair.customer, pair.billing))
        //                      .SelectMany(x => x)
        //                      .ToList();
        //    return batch;
        //}

        //internal ProcessBatch ProcessBatchGroupJoin(ProcessBatch batch)
        //{
        //    batch.Billings = batch.Customers
        //      .GroupJoin(batch.Billings, c => c.Cpf, b => b.Cpf, (c, b) => new { Customer = c, Billings = b })
        //      .SelectMany(x => x.Billings.Select(b => _processor.Process(x.Customer, b))).ToList();
        //    return batch;
        //}

        //internal ProcessBatch ProcessBatchJoin(ProcessBatch batch)
        //{
        //    batch.Billings = (from b in batch.Billings
        //                      join c in batch.Customers on b.Cpf equals c.Cpf
        //                      select _processor.Process(c, b)).ToList();
        //    return batch;
        //}
    }
}

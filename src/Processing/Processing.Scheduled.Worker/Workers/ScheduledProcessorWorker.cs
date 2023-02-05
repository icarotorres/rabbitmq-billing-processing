// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Library.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;

namespace Processing.Scheduled.Worker.Workers
{
    public class ScheduledProcessorWorker : BackgroundService
    {
        private readonly IBatchClient _batchClient;
        private readonly IAmountProcessor _processor;
        private readonly IComparer<ICpfCarrier> _comparer;
        private readonly ScheduledProcessorSettings _config;
        private readonly ILogger<ScheduledProcessorWorker> _logger;

        public int ErrorsCount { get; set; } = 0;

        public ScheduledProcessorWorker(
            IBatchClient batchClient,
            IAmountProcessor processor,
            IComparer<ICpfCarrier> comparer,
            ScheduledProcessorSettings config,
            ILogger<ScheduledProcessorWorker> logger = null)
        {
            _batchClient = batchClient;
            _processor = processor;
            _comparer = comparer;
            _config = config;
            _logger = logger;
        }

        // TODO: Extract this logic to another internal method to test it properly
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sw = Stopwatch.StartNew();
            await DoExecute();
            sw.Stop();
            if (sw.ElapsedMilliseconds < _config.MillisecondsScheduledTime)
            {
                Thread.Sleep(_config.MillisecondsScheduledTime - (int)sw.ElapsedMilliseconds);
            }
            sw.Reset();
            await StopAsync(stoppingToken);
            await StartAsync(stoppingToken);
        }

        internal async Task DoExecute()
        {
            _logger?.LogInformation($"Starting scheduled batch processing...");

            ErrorsCount = 0;
            var batch = new PairedBatch();
            do
            {
                try
                {
                    batch = await _batchClient.FetchBatchAsync(batch);
                    batch.BeProcessed(_processor, _comparer, _logger);
                    batch.Id = Guid.NewGuid().ToString();
                }
                catch (Exception ex)
                {
                    ErrorsCount++;
                    _logger?.LogError("BatchId: {BatchId}, ErrorCount: {ErrorCount}, Exceptions: {ExceptionMessages}",
                        batch.Id, ErrorsCount, string.Join(Environment.NewLine, ex.ExtractMessages()));
                }

            } while (_config.Retries > ErrorsCount);

            _logger?.LogError("exceeded max error count for this instance. ErrorCount: {ErrorCount}", ErrorsCount);
        }
    }
}

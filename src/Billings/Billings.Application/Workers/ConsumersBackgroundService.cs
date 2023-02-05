// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Billings.Application.Usecases;
using Billings.Domain.Models;
using Library.Messaging;
using Microsoft.Extensions.Hosting;

namespace Billings.Application.Workers
{
    public class ConsumersBackgroundService : BackgroundService
    {
        private readonly IMessageConsumer<ProcessedBatch> _processedBatchConsumer;

        public ConsumersBackgroundService(IMessageConsumer<ProcessedBatch> processedBatchConsumer)
        {
            _processedBatchConsumer = processedBatchConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processedBatchConsumer.ConsumeWithUsecase(nameof(ConfirmProcessedBatchUsecase));
        }
    }
}

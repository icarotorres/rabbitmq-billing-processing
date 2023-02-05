// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Library.Messaging;
using Microsoft.Extensions.Hosting;
using Processing.Eventual.Application.Usecases;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Application.Workers
{
    public class ConsumersBackgroundService : BackgroundService
    {
        private readonly IMessageConsumer<Billing> _billingIssuedConsumer;
        private readonly IMessageConsumer<ProcessedBatch> _batchProcessedConsumer;

        public ConsumersBackgroundService(IMessageConsumer<Billing> billingIssuedConsumer, IMessageConsumer<ProcessedBatch> batchConfirmedConsumer)
        {
            _billingIssuedConsumer = billingIssuedConsumer;
            _batchProcessedConsumer = batchConfirmedConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.WhenAll(
                _billingIssuedConsumer.ConsumeWithUsecase(nameof(HandleBillingIssuedUsecase)),
                _batchProcessedConsumer.ConsumeWithUsecase(nameof(HandleBatchConfirmedUsecase)));
        }
    }
}

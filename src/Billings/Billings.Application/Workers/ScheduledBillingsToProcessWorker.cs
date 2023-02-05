// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Billings.Application.Abstractions;
using Billings.Domain.Models;
using Library.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Billings.Application.Workers
{
    public class ScheduledBillingsToProcessWorker : RpcServer<List<Billing>>
    {
        private readonly IBillingRepository _repository;

        public ScheduledBillingsToProcessWorker(IConnectionFactory factory, IBillingRepository repository, ILogger<ScheduledBillingsToProcessWorker> logger) : base(nameof(Billing), factory, logger)
        {
            _repository = repository;
        }

        public override async Task<(List<Billing> receivedValue, string receivedMessage)> HandleReceivedMessage(BasicDeliverEventArgs ea)
        {
            var processedBatch = JsonSerializer.Deserialize<List<Billing>>(ea.Body.ToArray());
            await _repository.UpdateProcessedBatchAsync(processedBatch);
            return (processedBatch, BuildReceivedMessage(processedBatch).ToString());
        }

        public override async Task<string> WriteResponseMessage(List<Billing> receivedValue)
        {
            var pendingProcessing = await _repository.GetPendingAsync(default);
            return JsonSerializer.Serialize(pendingProcessing);
        }

        private StringBuilder BuildReceivedMessage(List<Billing> processedBatch)
        {
            var builderLength = Math.Max((processedBatch.Count * 2) + 1, 2);
            var idsMessageBuilder = new StringBuilder("billing ids received to process: [", builderLength);
            var enumerator = processedBatch.GetEnumerator();
            while (enumerator.Current is Billing billing)
            {
                if (billing != processedBatch[0])
                {
                    idsMessageBuilder.Append(",");
                }

                idsMessageBuilder.Append(enumerator.Current.Id);
                enumerator.MoveNext();
            }

            idsMessageBuilder.Append("]");
            return idsMessageBuilder;
        }
    }
}

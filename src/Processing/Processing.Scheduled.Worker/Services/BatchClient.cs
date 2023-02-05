// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Messaging;
using Microsoft.Extensions.Logging;
using Processing.Scheduled.Worker.Models;

namespace Processing.Scheduled.Worker.Services
{
    public class BatchClient : IBatchClient
    {
        private readonly IRpcClient<List<Customer>> _customerClient;
        private readonly IRpcClient<List<Billing>> _billingClient;
        private readonly ILogger<BatchClient> _logger;

        public BatchClient(IRpcClient<List<Customer>> customerClient, IRpcClient<List<Billing>> billingClient, ILogger<BatchClient> logger)
        {
            _customerClient = customerClient;
            _billingClient = billingClient;
            _logger = logger;
        }

        public async Task<PairedBatch> FetchBatchAsync(PairedBatch batch)
        {
            var billingTask = Task.Run(() =>
            {
                batch.Billings = _billingClient.CallProcedure(batch.Billings);
                _logger?.LogInformation("BatchId: {BatchId}. Subjects ready to process...", batch.Id);
            });
            var customerTask = Task.Run(() =>
            {
                batch.Customers = new List<ICpfCarrier>(_customerClient.CallProcedure(string.Empty));
                _logger?.LogInformation("BatchId: {BatchId}. Customers ready to process...", batch.Id);
            });

            await Task.WhenAll(billingTask, customerTask);
            return batch;
        }
    }
}

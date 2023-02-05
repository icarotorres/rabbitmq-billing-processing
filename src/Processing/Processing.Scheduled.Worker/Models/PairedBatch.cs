// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Processing.Scheduled.Worker.Services;

namespace Processing.Scheduled.Worker.Models
{
    public class PairedBatch
    {
        public PairedBatch()
        {
            Id = Guid.NewGuid().ToString();
            Billings = new List<Billing>();
            Customers = new List<ICpfCarrier>();
        }

        public string Id { get; set; }
        public List<Billing> Billings { get; set; }
        public List<ICpfCarrier> Customers { get; set; }

        public PairedBatch BeProcessed(IAmountProcessor processor, IComparer<ICpfCarrier> comparer, ILogger logger = null)
        {
            if (Customers.Count == 0 || Billings.Count == 0)
            {
                logger?.LogInformation("BatchId: {BatchId}. Skiping  Nothing to process now...", Id);
                return this;
            }

            logger?.LogInformation("BatchId: {BatchId}. Process started...", Id);
            Customers.Sort(comparer);
            Parallel.For(0, Billings.Count, billingIndex =>
            {
                var billing = Billings[billingIndex];
                var customerIndex = Customers.BinarySearch(billing, comparer);
                if (customerIndex >= 0)
                {
                    var customer = Customers[customerIndex];
                    Billings[billingIndex] = processor.Process(customer, billing);
                }
            });
            logger?.LogInformation("BatchId: {BatchId}. Process finished...", Id);

            return this;
        }
    }
}

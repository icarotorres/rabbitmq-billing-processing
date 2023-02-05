// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using UnitTests.Scheduled.Worker.Helpers;

namespace Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class ScheduledProcessorWorkerBehchmark
    {
        [Params(1000, 5000, 10000)] public int _customersCount;
        [Params(10, 50, 100)] public int _billingsPerCustomerCount;

        private PairedBatch _batch;
        private static readonly MathOnlyAmountProcessor s_processor = new MathOnlyAmountProcessor();
        private static readonly CpfCarrierComparer s_comparer = new CpfCarrierComparer();

        [IterationSetup]
        public void IterationSetup()
        {
            _batch = new PairedBatch
            {
                Customers = new List<ICpfCarrier>(InternalFakes.Customers.Valid().Generate(_customersCount))
            };
            _batch.Billings = _batch.Customers.SelectMany(x => InternalFakes.Billings.Valid(x.Cpf).Generate(_billingsPerCustomerCount)).ToList();
        }

        [Benchmark] public PairedBatch LinsIndexes() => _batch.BeProcessed(s_processor, s_comparer, null);
    }
}

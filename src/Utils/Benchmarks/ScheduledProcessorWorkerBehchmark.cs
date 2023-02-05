// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Processing.Scheduled.Worker;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using Processing.Scheduled.Worker.Workers;
using UnitTests.Scheduled.Worker.Helpers;

namespace Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class ScheduledProcessorWorkerBehchmark
    {
        [Params(1000)] public int _customersCount;
        [Params(100)] public int _billingsPerCustomerCount;
        private ScheduledProcessorWorker _worker;
        private List<ICpfCarrier> _customers;
        private ProcessBatch _batch;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var processor = new MathOnlyAmountProcessor();
            var comparer = new CpfCarrierComparer();
            var settings = new ScheduledProcessorSettings();
            _worker = new ScheduledProcessorWorker(default, default, processor, comparer, settings, default);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _customers = new List<ICpfCarrier>(InternalFakes.Customers.Valid().Generate(_customersCount));
            _batch = new ProcessBatch
            {
                Customers = _customers,
                Billings = _customers.SelectMany(x => InternalFakes.Billings.Valid(x.Cpf).Generate(_billingsPerCustomerCount)).ToList()
            };
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _customers = null;
            _batch = null;
        }

        [Benchmark] public ProcessBatch LinsIndexes() => _worker.ProcessBatch(_batch);

        //[Benchmark] public ProcessBatch Join() => _worker.ProcessBatchJoin(_batch);

        //[Benchmark] public ProcessBatch GroupJoin() => _worker.ProcessBatchGroupJoin(_batch);

        //[Benchmark] public ProcessBatch JoinGroupSelectMany() => _worker.ProcessBatchJoinGroupSelectMany(_batch);
    }
}

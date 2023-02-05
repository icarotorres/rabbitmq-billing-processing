// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Library.Messaging;
using Microsoft.Extensions.Logging;
using Moq;
using Processing.Scheduled.Worker;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using Processing.Scheduled.Worker.Workers;
using UnitTests.Scheduled.Worker.Helpers;
using Xunit;

namespace UnitTests.Scheduled.Worker.Workers
{
    [Trait("processing", "scheduled-worker")]
    public class ScheduledProcessorWorkerTests
    {
        private readonly Mock<IRpcClient<List<Customer>>> _customerClientMock;
        private readonly Mock<IRpcClient<List<Billing>>> _billingClientMock;
        private readonly Mock<IAmountProcessor> _amountProcessorMock;
        private readonly CpfCarrierComparer _comparer;
        private readonly Mock<ScheduledProcessorSettings> _settingsMock;
        private readonly Mock<ILogger<ScheduledProcessorWorker>> _loggerMock;
        private readonly ScheduledProcessorWorker _sut;

        public ScheduledProcessorWorkerTests()
        {
            _customerClientMock = new Mock<IRpcClient<List<Customer>>>();
            _billingClientMock = new Mock<IRpcClient<List<Billing>>>();
            _amountProcessorMock = new Mock<IAmountProcessor>();
            _comparer = new CpfCarrierComparer();
            _settingsMock = new Mock<ScheduledProcessorSettings>();
            _loggerMock = new Mock<ILogger<ScheduledProcessorWorker>>();
            _sut = new ScheduledProcessorWorker(
                _customerClientMock.Object,
                _billingClientMock.Object,
                _amountProcessorMock.Object,
                _comparer,
                _settingsMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task FetchBatch_Should_FetchCustomersAndBillings_WithCustomersSorted_ToMatchAndProcessAsync()
        {
            // arrange
            const int BillingsCount = 10;
            var batch = new ProcessBatch();
            var billings = InternalFakes.Billings.Valid().Generate(BillingsCount);
            var customers = new Customer[BillingsCount];
            var expectedCustomers = new ICpfCarrier[BillingsCount];
            for (var i = 0; i < BillingsCount; i++)
            {
                expectedCustomers[i] = customers[i] = new Customer { Cpf = billings[i].Cpf };
            }
            _billingClientMock.Setup(x => x.CallProcedure(batch.Billings)).Returns(billings);
            _customerClientMock.Setup(x => x.CallProcedure(string.Empty)).Returns(new List<Customer>(customers));

            // act
            var result = await _sut.FetchBatchAsync(batch);

            // assert
            result.Should().NotBeNull().And.BeOfType<ProcessBatch>();
            result.Billings.Should().NotBeNull()
                .And.HaveCount(BillingsCount)
                .And.BeEquivalentTo(billings);
            result.Customers.Should().NotBeNull()
                .And.HaveCount(BillingsCount)
                .And.BeEquivalentTo(expectedCustomers);
        }

        [Fact]
        public void ProcessBatch_Should_Return_BillingsProcessed_ForMatchingCustomers()
        {
            // arrange on constructor
            const int BillingsCount = 100;
            const int ProcessableCount = 20;
            var billings = InternalFakes.Billings.Valid().Generate(BillingsCount);
            var processableCpfs = new ICpfCarrier[ProcessableCount];
            for (var i = 0; i < ProcessableCount; i++)
            {
                processableCpfs[i] = new Customer { Cpf = billings[i].Cpf };
                _amountProcessorMock.Setup(y => y.Process(processableCpfs[i], billings[i])).Returns(FakeProcessBilling(billings[i]));
            }
            var unprocessableCpfs = billings.Except(processableCpfs, _comparer).ToList();
            var batch = new ProcessBatch { Customers = new List<ICpfCarrier>(processableCpfs), Billings = billings };

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Billings.Should()
                .HaveCount(BillingsCount)
                .And.OnlyContain(x =>
                    (unprocessableCpfs.Any(u => u.Cpf == x.Cpf) && x.ProcessedAt == null) ||
                    (processableCpfs.Any(p => p.Cpf == x.Cpf) && x.ProcessedAt != null))
                .And.Match(items => items.Count(x => x.ProcessedAt == null) == BillingsCount - ProcessableCount)
                .And.Match(items => items.Count(x => x.ProcessedAt != null) == ProcessableCount);
        }

        [Fact]
        public void ProcessBatch_Without_Customers_Should_ReturnInstance_And_SkipProcess()
        {
            // arrange on constructor
            const int BillingsCount = 2;
            var billings = InternalFakes.Billings.Valid().Generate(BillingsCount);
            var batch = new ProcessBatch { Customers = new List<ICpfCarrier>(), Billings = billings };

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull().And.Be(batch);
            result.Billings.Should().HaveCount(BillingsCount).And.OnlyContain(x => x.ProcessedAt == null);
        }

        //[Fact]
        //public void ProcessBatchJoin_Should_Return_BillingsProcessed_ForMatchingCustomers()
        //{
        //    // arrange on constructor
        //    const int BillingsCount = 100;
        //    const int ProcessableCount = 20;
        //    var billings = InternalFakes.Billings.Valid().Generate(BillingsCount);
        //    var processableCpfs = new ICpfCarrier[ProcessableCount];
        //    for (var i = 0; i < ProcessableCount; i++)
        //    {
        //        processableCpfs[i] = new Customer { Cpf = billings[i].Cpf };
        //        _amountProcessorMock.Setup(y => y.Process(processableCpfs[i], billings[i])).Returns(FakeProcessBilling(billings[i]));
        //    }
        //    var batch = new ProcessBatch { Customers = new List<ICpfCarrier>(processableCpfs), Billings = billings };

        //    // act
        //    var result = _sut.ProcessBatchJoin(batch);

        //    // assert
        //    result.Should().NotBeNull();
        //    result.Id.Should().NotBeNullOrEmpty();
        //    result.Billings.Should().HaveCount(ProcessableCount).And.OnlyContain(x => x.ProcessedAt != null);
        //}

        [Fact]
        public void ProcessBatch_Without_Billings_Should_ReturnInstance_And_SkipProcess()
        {
            // arrange on constructor
            const int CustomersCount = 2;
            var customers = InternalFakes.Customers.Valid().Generate(CustomersCount);
            var batch = new ProcessBatch { Customers = new List<ICpfCarrier>(customers), Billings = new List<Billing>() };

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull().And.Be(batch);
            result.Billings.Should().BeEmpty();
            result.Customers.Should().HaveCount(CustomersCount);
        }

        [Fact]
        public void ProcessBatch_Without_BillingsAndCustomers_Should_ReturnInstance_And_SkipProcess()
        {
            // arrange on constructor
            var batch = new ProcessBatch();

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull().And.Be(batch);
            result.Billings.Should().BeEmpty();
            result.Customers.Should().BeEmpty();
        }

        [Fact]
        public async Task DoExecute_Should_ExecuteProcess_ResetBatchId_And_ReturnProcessedBatch()
        {
            // arrange
            const int BillingsCount = 10;
            const int ExpectedDelay = 100;
            _settingsMock.Setup(x => x.MillisecondsScheduledTime).Returns(ExpectedDelay);
            var batch = new ProcessBatch();
            var billings = InternalFakes.Billings.Valid().Generate(BillingsCount);
            var customers = new Customer[BillingsCount];
            for (var i = 0; i < BillingsCount; i++)
            {
                customers[i] = new Customer { Cpf = billings[i].Cpf };
                _amountProcessorMock.Setup(y => y.Process(customers[i], billings[i])).Returns(FakeProcessBilling(billings[i]));
            }
            _billingClientMock.Setup(x => x.CallProcedure(batch.Billings)).Returns(billings);
            _customerClientMock.Setup(x => x.CallProcedure(string.Empty)).Returns(new List<Customer>(customers));

            // act
            var result = await _sut.DoExecute(batch);

            // assert
            result.Should().NotBeNull()
                .And.BeOfType<ProcessBatch>();
            result.Id.Should().NotBeNullOrEmpty();
            result.Customers.Should().NotBeNull()
                .And.BeEquivalentTo(customers);
            result.Billings.Should().NotBeNull()
                .And.BeEquivalentTo(billings)
                .And.HaveCount(BillingsCount)
                .And.OnlyContain(x => x.ProcessedAt != null && customers.Any(y => y.Cpf == x.Cpf));
        }

        private static Func<Billing> FakeProcessBilling(Billing billing) => () =>
        {
            billing.Amount = 100;
            billing.ProcessedAt = DateTime.UtcNow;
            return billing;
        };
    }
}

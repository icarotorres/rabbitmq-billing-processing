// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using UnitTests.Scheduled.Worker.Helpers;
using Xunit;

namespace UnitTests.Scheduled.Worker.Models
{
    [Trait("processing", "scheduled-worker-models")]
    public class PairedBatchTests
    {
        private readonly Mock<IAmountProcessor> _amountProcessorMock = new Mock<IAmountProcessor>();
        private readonly CpfCarrierComparer _comparer = new CpfCarrierComparer();
        private readonly PairedBatch _sut = new PairedBatch();

        [Theory]
        [InlineData(10, 2)]
        [InlineData(100, 20)]
        [InlineData(1000, 200)]
        public void BeProcessed_Should_Return_BillingsProcessed_ForMatchingCustomers(int billingsCount, int processableCount)
        {
            // arrange on constructor
            var billings = InternalFakes.Billings.Valid().Generate(billingsCount);
            var processableCpfs = new ICpfCarrier[processableCount];
            for (var i = 0; i < processableCount; i++)
            {
                var billing = billings[i];
                processableCpfs[i] = new Customer { Cpf = billing.Cpf };
                _amountProcessorMock.Setup(y => y.Process(processableCpfs[i], billing)).Returns(() =>
                {
                    billing.Amount = 100;
                    billing.ProcessedAt = DateTime.UtcNow;
                    return billing;
                });
            }
            var unprocessableCpfs = billings.Except(processableCpfs, _comparer).ToList();
            _sut.Billings = billings;
            _sut.Customers = new List<ICpfCarrier>(processableCpfs);

            // act
            var result = _sut.BeProcessed(_amountProcessorMock.Object, _comparer, null);

            // assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Billings.Should()
                .HaveCount(billingsCount)
                .And.OnlyContain(x =>
                    (unprocessableCpfs.Any(u => u.Cpf == x.Cpf) && x.ProcessedAt == null) ||
                    (processableCpfs.Any(p => p.Cpf == x.Cpf) && x.ProcessedAt != null))
                .And.Match(items => items.Count(x => x.ProcessedAt == null) == billingsCount - processableCount)
                .And.Match(items => items.Count(x => x.ProcessedAt != null) == processableCount);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(20)]
        [InlineData(200)]
        public void BeProcessed_Without_Customers_Should_ReturnInstance_And_SkipProcess(int billingsCount)
        {
            // arrange on constructor
            _sut.Billings = InternalFakes.Billings.Valid().Generate(billingsCount);

            // act
            var result = _sut.BeProcessed(_amountProcessorMock.Object, _comparer, null);

            // assert
            result.Should().NotBeNull().And.Be(_sut);
            result.Billings.Should().HaveCount(billingsCount).And.OnlyContain(x => x.ProcessedAt == null);
            result.Customers.Should().BeEmpty();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(20)]
        [InlineData(200)]
        public void BeProcessed_Without_Billings_Should_ReturnInstance_And_SkipProcess(int customersCount)
        {
            // arrange on constructor
            _sut.Customers = new List<ICpfCarrier>(InternalFakes.Customers.Valid().Generate(customersCount));

            // act
            var result = _sut.BeProcessed(_amountProcessorMock.Object, _comparer, null);

            // assert
            result.Should().NotBeNull().And.Be(_sut);
            result.Billings.Should().BeEmpty();
            result.Customers.Should().HaveCount(customersCount);
        }

        [Fact]
        public void BeProcessed_Without_BillingsAndCustomers_Should_ReturnInstance_And_SkipProcess()
        {
            // act
            var result = _sut.BeProcessed(_amountProcessorMock.Object, _comparer, null);

            // assert
            result.Should().NotBeNull().And.Be(_sut);
            result.Billings.Should().BeEmpty();
            result.Customers.Should().BeEmpty();
        }
    }
}

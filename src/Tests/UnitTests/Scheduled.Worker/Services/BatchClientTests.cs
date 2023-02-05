// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Library.Messaging;
using Moq;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using UnitTests.Scheduled.Worker.Helpers;
using Xunit;

namespace UnitTests.Scheduled.Worker.Services
{
    [Trait("processing", "scheduled-worker-services")]
    public class BatchClientTests
    {
        private readonly Mock<IRpcClient<List<Customer>>> _customerClientMock;
        private readonly Mock<IRpcClient<List<Billing>>> _billingClientMock;
        private readonly BatchClient _sut;

        public BatchClientTests()
        {
            _customerClientMock = new Mock<IRpcClient<List<Customer>>>();
            _billingClientMock = new Mock<IRpcClient<List<Billing>>>();
            _sut = new BatchClient(_customerClientMock.Object, _billingClientMock.Object, null);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(10, 1)]
        [InlineData(10, 10)]
        [InlineData(10, 100)]
        public async Task FetchBatch_Should_FetchCustomersAndBillings_ToMatchAndProcessAsync(int customersCount, int billingsPerCustomerCount)
        {
            // arrange
            var expectedBillingsCount = customersCount * billingsPerCustomerCount;
            var batch = new PairedBatch();
            var customers = InternalFakes.Customers.Valid().Generate(customersCount);
            var billings = new Billing[expectedBillingsCount];
            for (var i = 0; i < customers.Count; i++)
            {
                var offset = billingsPerCustomerCount * i;
                for (var j = 0; j < billingsPerCustomerCount; j++)
                {
                    billings[offset + j] = InternalFakes.Billings.Valid(customers[i].Cpf).Generate();
                }
            }

            _billingClientMock.Setup(x => x.CallProcedure(batch.Billings)).Returns(new List<Billing>(billings));
            _customerClientMock.Setup(x => x.CallProcedure(string.Empty)).Returns(customers);

            // act
            var result = await _sut.FetchBatchAsync(batch);

            // assert
            result.Should().NotBeNull().And.BeOfType<PairedBatch>();
            result.Billings.Should().NotBeNull()
                .And.HaveCount(expectedBillingsCount)
                .And.BeEquivalentTo(billings);
            result.Customers.Should().NotBeNull()
                .And.HaveCount(customersCount)
                .And.BeEquivalentTo(customers);
        }
    }
}

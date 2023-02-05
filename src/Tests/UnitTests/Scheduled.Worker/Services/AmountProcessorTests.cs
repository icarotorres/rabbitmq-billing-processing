// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using UnitTests.Scheduled.Worker.Helpers;
using Xunit;

namespace UnitTests.Scheduled.Worker.Services
{
    [Trait("processing", "scheduled-worker-services")]
    public class AmountProcessorTests
    {
        [Fact]
        public void MathOnlyAmountProcessor_Calculate()
        {
            // arrange
            var customer = InternalFakes.Customers.Valid().Generate();
            var billing = InternalFakes.Billings.Valid(customer.Cpf).Generate();
            var str = customer.Cpf.ToString();
            var processedAtValueBeforeProcess = billing.ProcessedAt;
            var digits = new char[4] { str[0], str[1], str[^2], str[^1] };
            var expectedAmount = decimal.Parse(digits);
            var sut = new MathOnlyAmountProcessor();

            // act
            var result = sut.Process(customer, billing);

            // assert
            result.Should().NotBeNull()
                .And.BeOfType<Billing>()
                .And.BeSameAs(billing);
            result.Amount.Should().Be(expectedAmount);
            result.Amount.ToString("0000").Should().Be(string.Join(string.Empty, digits));
            result.ProcessedAt.Should().NotBeNull();
            processedAtValueBeforeProcess.Should().BeNull();
        }
    }
}

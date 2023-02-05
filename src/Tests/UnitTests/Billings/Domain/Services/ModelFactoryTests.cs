// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Billings.Domain.Models;
using Billings.Domain.Services;
using Bogus;
using FluentAssertions;
using Library.Results;
using Library.TestHelpers;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Domain.Services
{
    [Trait("billings", "domain")]
    public class ModelFactoryTests
    {
        [Fact]
        public void CreateBilling_Should_Create_Billing_With_PropsMatching_InUpperCase()
        {
            // arrange
            var sut = new ModelFactory();
            var expectedCpf = Fakes.CPFs.Valid().Generate();
            var expectedAmount = new Faker().Random.Double(10, 10000);
            var expectedDate = InternalFakes.Dates.FutureDay(5).Generate();

            // act
            var result = sut.CreateBilling(expectedCpf.ToString(), expectedAmount, expectedDate.ToString());

            // assert
            result.Should().NotBeNull().And.BeOfType<Billing>().And.NotBeAssignableTo<INull>();
            result.Id.Should().NotBeEmpty();
            result.Cpf.Should().Be(expectedCpf.Value);
            result.Amount.Should().Be(expectedAmount);
            result.DueDate.Should().NotBeNull();
            result.DueDate.ToString().Should().Be(expectedDate.ToString());
        }
    }
}

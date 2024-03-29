// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Customers.Domain.Models;
using FluentAssertions;
using Library.Results;
using Xunit;

namespace UnitTests.Customers.Domain.Models
{
    [Trait("customers", "domain")]
    public class NullCustomerTests
    {
        [Fact]
        public void Constructor_Should_Create_With_EmptyProps()
        {
            // arrange and act
            var sut = new NullCustomer();

            // assert
            sut.Should().NotBeNull().And.NotBeOfType<Customer>().And.BeAssignableTo<INull>();
            sut.Cpf.Should().Be(0);
            sut.Name.Should().BeNullOrEmpty();
            sut.State.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Constructor_AssigningValues_Should_Create_With_EmptyProps()
        {
            // arrange and act
            var sut = new NullCustomer
            {
                Name = "sample",
                State = "sample",
                Cpf = 01234567890
            };

            sut.Should().NotBeNull().And.NotBeOfType<Customer>().And.BeAssignableTo<INull>();
            sut.Cpf.Should().Be(0);
            sut.Name.Should().BeNullOrEmpty();
            sut.State.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Instance_Should_Have_ImmutableProps()
        {
            // arrange and act
            var sut = new NullCustomer
            {
                Name = "sample",
                State = "sample",
                Cpf = 01234567890
            };

            sut.Should().NotBeNull().And.NotBeOfType<Customer>().And.BeAssignableTo<INull>();
            sut.Cpf.Should().Be(0);
            sut.Name.Should().BeNullOrEmpty();
            sut.State.Should().BeNullOrEmpty();
        }
    }
}

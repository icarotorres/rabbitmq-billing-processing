// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Billings.Domain.Models;
using FluentAssertions;
using Library.Results;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Domain.Models
{
    [Trait("billings", "domain")]
    public class DateTests
    {
        [Fact]
        public void Constructor_Should_Create_With_EmptyProps()
        {
            // arrange and act
            var sut = new Date();

            // assert
            sut.Should().NotBeNull().And.BeOfType<Date>().And.NotBeAssignableTo<INull>();
            sut.Day.Should().Be(0);
            sut.Month.Should().Be(0);
            sut.Year.Should().Be(0);
            sut.ToString().Should().Be($"{sut.Day:00}-{sut.Month:00}-{sut.Year:0000}");
        }

        [Fact]
        public void Constructor_AssigningValues_Should_Create_With_Props_For_GivenValues()
        {
            // arrange
            var expectedDateValues = InternalFakes.Dates.FutureDay(1).Generate();

            // act
            var sut = new Date
            {
                Day = expectedDateValues.Day,
                Month = expectedDateValues.Month,
                Year = expectedDateValues.Year,
            };

            sut.Should().NotBeNull().And.BeOfType<Date>().And.NotBeAssignableTo<INull>();
            sut.Day.Should().Be(expectedDateValues.Day);
            sut.Month.Should().Be(expectedDateValues.Month);
            sut.Year.Should().Be(expectedDateValues.Year);
            sut.ToString().Should().Be($"{sut.Day:00}-{sut.Month:00}-{sut.Year:0000}");
        }

        [Fact]
        public void Constructor_WithParam_Should_Create_With_Props_For_GivenValues()
        {
            // arrange
            var expectedDateValues = InternalFakes.Dates.FutureDay(1).Generate();

            // act
            var sut = new Date(expectedDateValues.ToString());

            sut.Should().NotBeNull().And.BeOfType<Date>().And.NotBeAssignableTo<INull>();
            sut.Day.Should().Be(expectedDateValues.Day);
            sut.Month.Should().Be(expectedDateValues.Month);
            sut.Year.Should().Be(expectedDateValues.Year);
            sut.ToString().Should().Be($"{sut.Day:00}-{sut.Month:00}-{sut.Year:0000}");
        }

        [Fact]
        public void Instance_Should_Have_Mutable_PropValues()
        {
            // arrange
            var sut = new Date();
            var previousDay = sut.Day;
            var previousMonth = sut.Month;
            var previousYear = sut.Year;
            var expectedDateValues = InternalFakes.Dates.FutureDay(1).Generate();

            // act
            sut.Day = expectedDateValues.Day;
            sut.Month = expectedDateValues.Month;
            sut.Year = expectedDateValues.Year;

            sut.Should().NotBeNull().And.BeOfType<Date>().And.NotBeAssignableTo<INull>();
            sut.Day.Should().Be(expectedDateValues.Day).And.NotBe(previousDay);
            sut.Month.Should().Be(expectedDateValues.Month).And.NotBe(previousMonth);
            sut.Year.Should().Be(expectedDateValues.Year).And.NotBe(previousYear);
            sut.ToString().Should().Be($"{sut.Day:00}-{sut.Month:00}-{sut.Year:0000}");
        }
    }
}

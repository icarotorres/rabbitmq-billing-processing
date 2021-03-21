﻿using FluentAssertions;
using Issuance.Api.Application.Models;
using Issuance.Api.Application.Usecases;
using Issuance.Api.UnitTests.Helpers;
using Library.Results;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace Issuance.Api.UnitTests.Application.Usecases
{
    [Trait("unit-test", "issuance.api-application")]
    public class IssuanceUsecaseTests
    {
        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommand()
        {
            // arrange
            var request = InternalFakes.BillingRequests.Valid().Generate();
            const int expectedStatus = StatusCodes.Status201Created;
            var expectedBilling = InternalFakes.Billings.Valid().Generate();
            var factory = ModelFactoryMockBuilder.Create().CreateBilling(
                cpfString: request.Cpf,
                amount: request.Amount,
                dueDate: request.DueDate,
                model: expectedBilling).Build();

            var repository = BillingRepositoryMockBuilder.Create()
                .Insert(expectedBilling, Task.CompletedTask).Build();

            var sut = new IssuanceUsecase(factory, repository);

            // act
            var result = await sut.Handle(request, default);
            var resultData = (BillingResponse)result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(expectedStatus);
            resultData.Should().NotBeNull().And.BeOfType<BillingResponse>();
            resultData.Id.Should().Be(expectedBilling.Id);
            resultData.Cpf.Should().Be(expectedBilling.Cpf.ToString("00000000000"));
            resultData.Amount.Should().Be(expectedBilling.Amount);
            resultData.DueDate.Should().Be(expectedBilling.DueDate.ToString());
        }
    }
}
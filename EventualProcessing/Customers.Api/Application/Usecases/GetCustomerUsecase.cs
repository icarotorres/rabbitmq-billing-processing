﻿using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Optmizations;
using PrivatePackage.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Usecases
{
    public class GetCustomerUsecase : IRequestHandler<GetCustomerRequest, IResult>
    {
        private readonly ICustomerRepository repository;

        public GetCustomerUsecase(ICustomerRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IResult> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
            var id = request.Cpf.AsSpan().ParseUlong();
            var customer = await repository.GetAsync(id, cancellationToken);
            return new SuccessResult(customer);
        }
    }
}

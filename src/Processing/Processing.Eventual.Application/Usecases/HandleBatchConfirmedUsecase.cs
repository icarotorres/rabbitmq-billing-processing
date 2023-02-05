// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Library.Results;
using MediatR;
using Processing.Eventual.Application.Abstractions;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Application.Usecases
{
    public class HandleBatchConfirmedUsecase : IRequestHandler<ProcessedBatch, IResult>
    {
        private readonly IBillingsRepository _repository;

        public HandleBatchConfirmedUsecase(IBillingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IResult> Handle(ProcessedBatch request, CancellationToken cancellationToken)
        {
            await _repository.RemoveManyConfirmedAsync(request, cancellationToken);
            return new SuccessResult(request);
        }
    }
}

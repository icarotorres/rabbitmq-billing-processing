﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Billings.Application.Abstractions;
using Billings.Domain.Models;
using Library.Messaging;
using Library.Results;
using MediatR;

namespace Billings.Application.Usecases
{
    public class ConfirmProcessedBatchUsecase : IRequestHandler<ProcessedBatch, IResult>
    {
        private readonly IBillingRepository _repository;
        private readonly IMessagePublisher _publisher;

        public ConfirmProcessedBatchUsecase(IBillingRepository repository, IMessagePublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<IResult> Handle(ProcessedBatch request, CancellationToken cancellationToken)
        {
            await _repository.UpdateProcessedBatchAsync(request, cancellationToken);
            /*
             * Sem confirmação a título de propótipo. Use BasicConfirmedMessage se deseja confirmações de publicação
             */
            await _publisher.Publish(new BasicMessage(request, nameof(ConfirmProcessedBatchUsecase)));
            return new SuccessResult(request);
        }
    }
}

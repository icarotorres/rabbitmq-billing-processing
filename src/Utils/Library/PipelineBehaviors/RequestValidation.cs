// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Library.Results;
using MediatR;

namespace Library.PipelineBehaviors
{
    public class RequestValidation<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
      where TRequest : IRequest<TResult>
      where TResult : IResult
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidation(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var tasks = _validators.Select(x => x.ValidateAsync(context, cancellationToken));
            var results = await Task.WhenAll(tasks);
            var failures = results.SelectMany(x => x.Errors).Where(x => x != null).ToList();

            return failures.Count > 0
                ? (TResult)(IResult)new FailResult(failures)
                : await next();
        }
    }
}

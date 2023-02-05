// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Library.PipelineBehaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Processing.Eventual.Domain.Services;

namespace Processing.Eventual.Worker.DependencyInjection
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection BootstrapPipelinesServices(this IServiceCollection services)
        {
            return services
                // mediatR dependency injection
                .AddTransient<IAmountProcessor, MathOnlyAmountProcessor>()
                .AddMediatR(Assembly.GetExecutingAssembly())

                // mediatR pre-request open-type pipeline behaviors
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidation<,>));
        }
    }
}

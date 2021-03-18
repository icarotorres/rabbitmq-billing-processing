﻿using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Services;
using Issuance.Api.Application.Usecases;
using Library.PipelineBehaviors;
using MediatR;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection BootstrapPipelinesServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IResponseConverter, ResponseConverter>()

                // mediatR dependency injection
                .AddMediatR(typeof(IssuanceUsecase).GetTypeInfo().Assembly)

                // mediatR pre-request open-type pipeline behaviors
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidation<,>));
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Billings.Application.Workers;
using Library.Configurations;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WorkerExtensions
    {
        public static IServiceCollection BootstrapWorkerServices(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQ = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
            return services
                .AddSingleton(rabbitMQ)
                .AddSingleton<IConnectionFactory>(_ => new ConnectionFactory { Uri = new Uri(rabbitMQ.AmqpUrl) })
                .AddHostedService<ScheduledBillingsToProcessWorker>();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Library.Configurations;
using Library.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessagingExtensions
    {
        public static IServiceCollection BootstrapMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQ = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMQ.AmqpUrl),
                DispatchConsumersAsync = rabbitMQ.DispatchConsumersAsync,
                AutomaticRecoveryEnabled = rabbitMQ.AutomaticRecoveryEnabled
            };
            services
                .AddSingleton(rabbitMQ)
                .AddSingleton<IConnectionFactory, ConnectionFactory>(_ => factory);

            if (rabbitMQ.PublishExchanges != null)
            {
                services.AddSingleton<IMessagePublisher, PublisherRabbitMQ>(x =>
                {
                    var connection = factory.CreateConnection();
                    var logger = x.GetRequiredService<ILogger<PublisherRabbitMQ>>();
                    return new PublisherRabbitMQ(connection, rabbitMQ, logger);
                });
            }
            if (rabbitMQ.ConsumeExchanges != null)
            {
                services.AddSingleton(_ => factory.CreateConnection())
                       .AddSingleton(typeof(IMessageConsumer<>), typeof(ConsumerRabbitMQ<>));
            }

            return services;
        }
    }
}

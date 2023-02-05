// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Library.Messaging
{
    public interface IRpcServer<T> : IHostedService, IDisposable
    {
        EventHandler<BasicDeliverEventArgs> OnMessageReceived { get; }
        EventingBasicConsumer BuildConsumer(string queueName, IConnectionFactory connectionFactory);
        IBasicProperties CreateBasicProperties(BasicDeliverEventArgs ea);
        Task<(T receivedValue, string receivedMessage)> HandleReceivedMessage(BasicDeliverEventArgs ea);
        Task<string> WriteResponseMessage(T receivedValue);
    }
}

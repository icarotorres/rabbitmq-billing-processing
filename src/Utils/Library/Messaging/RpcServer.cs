// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Library.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Library.Messaging
{
    public abstract class RpcServer<T> : BackgroundService, IRpcServer<T>
    {
        public EventHandler<BasicDeliverEventArgs> OnMessageReceived => OnMessageReceivedEventHandler;
        protected readonly EventingBasicConsumer Consumer;
        protected readonly ILogger<RpcServer<T>> Logger;

        protected RpcServer(string queueName, IConnectionFactory connectionFactory, ILogger<RpcServer<T>> logger)
        {
            Logger = logger;
            Consumer = BuildConsumer(queueName, connectionFactory);
        }

        public virtual EventingBasicConsumer BuildConsumer(string queueName, IConnectionFactory connectionFactory)
        {
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Logger.LogInformation("Awaiting RPC requests for queue {Queue}", queueName);
            return consumer;
        }

        public virtual IBasicProperties CreateBasicProperties(BasicDeliverEventArgs ea)
        {
            var replyProperties = Consumer.Model.CreateBasicProperties();
            replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;
            return replyProperties;
        }

        public abstract Task<(T receivedValue, string receivedMessage)> HandleReceivedMessage(BasicDeliverEventArgs ea);

        public abstract Task<string> WriteResponseMessage(T receivedValue);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Consumer.Received += OnMessageReceived;
            return Task.CompletedTask;
        }

        protected async void OnMessageReceivedEventHandler(object model, BasicDeliverEventArgs ea)
        {
            OnMessageReceivedStarts(ea);
            var responseMessage = string.Empty;
            var replyProperties = CreateBasicProperties(ea);
            try
            {
                var (receivedValue, receivedMessage) = await HandleReceivedMessage(ea);
                OnMessageReaded(ea, receivedMessage);
                responseMessage = await WriteResponseMessage(receivedValue);
                OnResponseWritten(ea, responseMessage);
            }
            catch (Exception ex)
            {
                OnMessageReceivedException(ea, ex);
            }
            finally
            {
                OnMessageReceivedEnds(ea, Consumer.Model, responseMessage, ea.BasicProperties, replyProperties);
            }
        }

        protected virtual void OnMessageReceivedStarts(BasicDeliverEventArgs ea)
        {
            Logger.LogInformation("Received on CorrelationId: {CorrelationId}, RoutingKey: {RoutingKey}, DeliveryTag: {DeliveryTag}.",
                ea.BasicProperties.CorrelationId, ea.RoutingKey, ea.DeliveryTag);
        }

        protected virtual void OnMessageReaded(BasicDeliverEventArgs ea, string readMessage)
        {
            Logger.LogInformation("Readed on CorrelationId: {CorrelationId}, RoutingKey: {RoutingKey}, DeliveryTag: {DeliveryTag}, Message: {Message}.",
                ea.BasicProperties.CorrelationId, ea.RoutingKey, ea.DeliveryTag, readMessage);
        }

        protected virtual void OnResponseWritten(BasicDeliverEventArgs ea, string responseMessage)
        {
            Logger.LogInformation("Readed on CorrelationId: {CorrelationId}, RoutingKey: {RoutingKey}, DeliveryTag: {DeliveryTag}, ReplyMessage: {ReplyMessage}.",
                ea.BasicProperties.CorrelationId, ea.RoutingKey, ea.DeliveryTag, responseMessage);
        }

        protected virtual void OnMessageReceivedException(BasicDeliverEventArgs ea, Exception ex)
        {
            var errors = string.Join(Environment.NewLine, ex.ExtractMessages());
            Logger.LogInformation("Readed on CorrelationId: {CorrelationId}, RoutingKey: {RoutingKey}, DeliveryTag: {DeliveryTag}, Errors: {Errors}.",
                ea.BasicProperties.CorrelationId, ea.RoutingKey, ea.DeliveryTag, errors);
        }

        protected virtual void OnMessageReceivedEnds(BasicDeliverEventArgs ea, IModel channel, string response, IBasicProperties receivedProperties, IBasicProperties replyProperties)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: string.Empty, routingKey: receivedProperties.ReplyTo, basicProperties: replyProperties, body: responseBytes);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;
using Library.Configurations;
using Library.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Library.Messaging
{
    public class ConsumerRabbitMQ<T> : IMessageConsumer<T> where T : IRequest<IResult>
    {
        private readonly IConnection _connection;
        private readonly RabbitMQSettings _settings;
        private readonly IMediator _mediator;
        private readonly ILogger<ConsumerRabbitMQ<T>> _logger;

        public ConsumerRabbitMQ(
            IConnection connection,
            RabbitMQSettings settings,
            IMediator mediator,
            ILogger<ConsumerRabbitMQ<T>> logger)
        {
            _connection = connection;
            _mediator = mediator;
            _logger = logger;
            _settings = settings;
        }

        public async Task ConsumeWithUsecase(string consumerUsecase)
        {
            await Task.Run(() =>
            {
                var channel = _connection.CreateModel();
                var exchangeSettings = _settings.ConsumeExchanges.GetSettings(consumerUsecase);

                channel.ExchangeDeclare(exchange: exchangeSettings.Name, type: exchangeSettings.Type);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: exchangeSettings.Name, routingKey: exchangeSettings.RoutingKey);

                if (_settings.DispatchConsumersAsync)
                {
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) => await OnMessageReceived(channel, consumerUsecase, queueName, ea);
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                }
                else
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) => await OnMessageReceived(channel, consumerUsecase, queueName, ea);
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                }
            });
        }

        private async Task OnMessageReceived(IModel channel, string consumerName, string queueName, BasicDeliverEventArgs ea)
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var usecaseResult = await SendMessageToUsecase(body);
            if (usecaseResult.IsSuccess())
            {
                _logger.LogInformation("Message consumed. RoutingKey: {RoutingKey}, Consumer: {consumerName}, Queue: {Queue}, DeliveryTag: {DeliveryTag}, Body: {Body}.",
                    ea.RoutingKey, consumerName, queueName, ea.DeliveryTag, body);
                channel.BasicAck(ea.DeliveryTag, false);
            }
            else
            {
                var errorLines = string.Join(Environment.NewLine, usecaseResult.Errors);
                _logger.LogError("Message consumption failed. RoutingKey: {RoutingKey}, Consumer: {consumerName}, Queue: {Queue}, DeliveryTag: {DeliveryTag}, Body: {Body}, Errors: {Errors}.",
                    ea.RoutingKey, consumerName, queueName, ea.DeliveryTag, body, errorLines);
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        private async Task<IResult> SendMessageToUsecase(string stringMessage)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<T>(stringMessage, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });
                return await _mediator.Send(message);
            }
            catch (Exception ex)
            {
                return new FailResult(StatusCodes.Status500InternalServerError, ex.ExtractMessages());
            }
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Library.Messaging
{

    public class RpcClient<T> : IRpcClient<T>
    {
        private readonly IModel _channel;
        private readonly string _targetQueueName;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly BlockingCollection<T> _responseData = new BlockingCollection<T>();
        private IBasicProperties _properties;

        public RpcClient(IModel channel, string targetQueueName)
        {
            _channel = channel;
            _targetQueueName = targetQueueName;
            _replyQueueName = channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(channel);
        }

        public T CallProcedure(object payload)
        {
            var message = JsonConvert.SerializeObject(payload);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            BuildMessageProperties();
            _consumer.Received += OnMessageReceived;

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _targetQueueName,
                basicProperties: _properties,
                body: messageBytes);

            _channel.BasicConsume(
                queue: _replyQueueName,
                autoAck: true,
                consumer: _consumer);

            return _responseData.Take();
        }

        internal void BuildMessageProperties()
        {
            _properties = _channel.CreateBasicProperties();
            _properties.CorrelationId = Guid.NewGuid().ToString();
            _properties.ReplyTo = _replyQueueName;
        }

        public void OnMessageReceived(object model, BasicDeliverEventArgs ea)
        {
            if (ea.BasicProperties.CorrelationId == _properties.CorrelationId)
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                var data = JsonConvert.DeserializeObject<T>(response);
                _responseData.Add(data);
            }
        }
    }
}

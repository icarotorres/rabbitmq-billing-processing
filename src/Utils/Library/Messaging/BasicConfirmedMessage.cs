// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Library.Messaging
{
    /// <summary>
    /// Pre-configured message enabling publisher acknowledgements
    /// </summary>
    public class BasicConfirmedMessage : MessageBase
    {
        public BasicConfirmedMessage(object payload, string publisherName) : base(payload, publisherName)
        {
        }
        /// <inheritdoc/>
        internal override void ConfigureConfirmation(
            IModel channel,
            string routingKey,
            string message,
            ConcurrentDictionary<ulong, string> outstandingConfirms,
            ILogger<IMessagePublisher> logger)
        {
            channel.ConfirmSelect();
            outstandingConfirms.TryAdd(channel.NextPublishSeqNo, message);
            RegisterEventCallbacks(channel, routingKey, outstandingConfirms, logger);
        }

        private void RegisterEventCallbacks(
            IModel channel, string routingKey,
            ConcurrentDictionary<ulong, string> outstandingConfirms,
            ILogger<IMessagePublisher> logger)
        {
            channel.BasicAcks += (sender, ea) =>
            {
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out var body);
                logger.LogInformation($"Message ack-ed. Publisher: {Publisher}, RoutingKey: {routingKey}, DeliveryTag: {ea.DeliveryTag}, Multiple: {ea.Multiple}, Body: {body}");
                CleanOutstandingConfirms(outstandingConfirms, ea.DeliveryTag, ea.Multiple);
            };
            channel.BasicNacks += (sender, ea) =>
            {
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out var body);
                logger.LogWarning($"Message nack-ed. Publisher: {Publisher}, RoutingKey: {routingKey}, DeliveryTag: {ea.DeliveryTag}, multiple: {ea.Multiple}, body: {body}");
                CleanOutstandingConfirms(outstandingConfirms, ea.DeliveryTag, ea.Multiple);
            };
        }

        private void CleanOutstandingConfirms(ConcurrentDictionary<ulong, string> outstandingConfirms, ulong deliveryTag, bool multiple)
        {
            if (!multiple)
            {
                outstandingConfirms.TryRemove(deliveryTag, out _);
                return;
            }

            foreach (var entry in outstandingConfirms)
            {
                if (entry.Key <= deliveryTag)
                {
                    outstandingConfirms.TryRemove(entry.Key, out _);
                }
            }
        }
    }
}

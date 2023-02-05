// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Library.Messaging
{
    /// <summary>
    /// Base configured message
    /// </summary>
    public abstract class MessageBase
    {
        protected MessageBase(object payload, string publisher)
        {
            Payload = payload;
            Publisher = publisher;
        }
        public virtual object Payload { get; }
        public virtual string Publisher { get; }

        /// <summary>
        /// Overridable method configuring publisher confirmation step.
        /// </summary>
        /// <param name="channel">Channel used for publishing message.</param>
        /// <param name="routingKey">The targeted routing key.</param>
        /// <param name="message">The serialized message to send.</param>
        /// <param name="outstandingConfirms">
        /// Dictionary mapping <see cref="IModel.NextPublishSeqNo"/> to serialized <see cref="Payload"/> string.
        /// </param>
        /// <param name="logger">Logger allowing personalized logs.</param>
        internal abstract void ConfigureConfirmation(
            IModel channel,
            string routingKey,
            string message,
            ConcurrentDictionary<ulong, string> outstandingConfirms,
            ILogger<IMessagePublisher> logger);
    }
}

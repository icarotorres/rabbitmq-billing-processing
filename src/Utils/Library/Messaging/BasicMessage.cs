// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Library.Messaging
{
    /// <summary>
    /// Message without publisher acknowledgements
    /// </summary>
    public class BasicMessage : MessageBase
    {
        public BasicMessage(object payload, string publisher) : base(payload, publisher)
        {
        }
        /// <inheritdoc/>
        internal override void ConfigureConfirmation(IModel channel, string routingKey, string message, ConcurrentDictionary<ulong, string> outstandingConfirms, ILogger<IMessagePublisher> logger)
        {
        }
    }
}

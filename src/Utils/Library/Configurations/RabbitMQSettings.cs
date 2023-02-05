// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Library.Configurations
{
    public sealed class RabbitMQSettings
    {
        public string AmqpUrl { get; set; }
        public bool DispatchConsumersAsync { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public ExchangeDictionary PublishExchanges { get; set; }
        public ExchangeDictionary ConsumeExchanges { get; set; }
    }
}

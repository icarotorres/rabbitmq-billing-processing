// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Library.Messaging;
using Moq;

namespace UnitTests.Customers.Helpers
{
    public sealed class MessagePublisherMockBuilder
    {
        private readonly Mock<IMessagePublisher> _mock;

        private MessagePublisherMockBuilder()
        {
            _mock = new Mock<IMessagePublisher>();
            _mock.Setup(x => x.Publish(It.IsAny<MessageBase>())).Returns(Task.CompletedTask);
        }

        public static MessagePublisherMockBuilder Create()
        {
            return new MessagePublisherMockBuilder();
        }

        public IMessagePublisher Build()
        {
            return _mock.Object;
        }
    }
}

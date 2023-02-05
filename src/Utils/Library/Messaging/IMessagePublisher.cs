// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace Library.Messaging
{
    /// <summary>
    /// Messaging Publisher abstraction accepting messages implementing <see cref="MessageBase"/>.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes configured messages implementing <see cref="MessageBase"/>.
        /// </summary>
        /// <param name="messageSettings"></param>
        Task Publish(MessageBase messageSettings);
    }
}

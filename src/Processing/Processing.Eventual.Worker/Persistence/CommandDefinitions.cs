// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MongoDB.Driver;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Worker.Persistence
{
    public static class CommandDefinitions
    {
        public static UpdateDefinition<Billing> SetProcessed(Billing entity)
        {
            return Builders<Billing>.Update
                .Set(x => x.ProcessedAt, entity.ProcessedAt)
                .Set(x => x.Amount, entity.Amount);
        }
    }
}

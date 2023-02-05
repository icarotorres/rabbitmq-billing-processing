// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Billings.Domain.Models;
using Library.Configurations;
using MongoDB.Driver;

namespace Billings.Infrastructure.Persistence.Services
{
    public class BillingsContext : IBillingsContext
    {
        public BillingsContext(IMongoDatabase database, CollectionsDictionary collectionsDictionary)
        {
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Billing> Billings { get; }
    }
}

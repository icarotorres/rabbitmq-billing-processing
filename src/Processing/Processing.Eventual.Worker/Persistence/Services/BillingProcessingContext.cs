// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Library.Configurations;
using MongoDB.Driver;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Worker.Persistence.Services
{
    public class BillingProcessingContext : IBillingProcessingContext
    {
        public BillingProcessingContext(IMongoDatabase database, CollectionsDictionary collectionsDictionary)
        {
            Customers = database.GetCollection<Customer>(collectionsDictionary.GetCollectionName(nameof(Customer)));
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Customer> Customers { get; }
        public IMongoCollection<Billing> Billings { get; }
    }
}

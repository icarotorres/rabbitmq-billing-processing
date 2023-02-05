// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MongoDB.Driver;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Worker.Persistence
{
    public static class QueryFilters
    {
        public static FilterDefinition<Billing> BillingsByCustomerCpf(ulong cpf)
        {
            return Builders<Billing>.Filter.Eq(x => x.Cpf, cpf);
        }

        public static FilterDefinition<Billing> BillingsPending()
        {
            return Builders<Billing>.Filter.Eq(x => x.ProcessedAt, null);
        }

        public static FilterDefinition<Billing> BillingsProcessed()
        {
            return Builders<Billing>.Filter.Ne(x => x.ProcessedAt, null);
        }

        public static FilterDefinition<Billing> BillingById(string id)
        {
            return Builders<Billing>.Filter.Eq(x => x.Id, id);
        }

        public static FilterDefinition<Billing> BillingIdIn(IEnumerable<string> ids)
        {
            return Builders<Billing>.Filter.In(x => x.Id, ids);
        }

        public static FilterDefinition<Customer> CustomerByCpf(ulong cpf)
        {
            return Builders<Customer>.Filter.Eq(x => x.Cpf, cpf);
        }
    }
}

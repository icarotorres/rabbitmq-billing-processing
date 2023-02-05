// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Processing.Eventual.Application.Abstractions;
using Processing.Eventual.Domain.Models;
using Processing.Eventual.Worker.Persistence.Services;

namespace Processing.Eventual.Worker.Persistence
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IBillingProcessingContext _context;

        public CustomerRepository(IBillingProcessingContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetAsync(ulong cpf, CancellationToken token)
        {
            return await _context.Customers.Find(QueryFilters.CustomerByCpf(cpf))
                                        .FirstOrDefaultAsync(token) ?? Customer.Null;
        }

        public async Task InsertAsync(Customer entity, CancellationToken token)
        {
            await _context.Customers.InsertOneAsync(entity, cancellationToken: token);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Customers.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence
{
    public class CustomersContext : DbContext
    {
        public CustomersContext(DbContextOptions<CustomersContext> options) : base(options)
        {
        }

        /// <summary>
        /// For mock purposes
        /// </summary>
        public CustomersContext()
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<NullCustomer>();
            modelBuilder.ApplyConfiguration(new CustomerMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Customers.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customers.Infrastructure.Persistence
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Cpf);
            builder.HasIndex(u => u.Cpf).IsUnique();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.State).IsRequired();
        }
    }
}

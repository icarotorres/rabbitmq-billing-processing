// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Customers.Domain.Models;
using Library.Optimizations;

namespace Customers.Domain.Services
{
    /// <inheritdoc cref="IModelFactory"/>
    public class ModelFactory : IModelFactory
    {
        public Customer CreateCustomer(string cpf, string name, string state)
        {
            return new Customer
            {
                Cpf = cpf.AsSpan().ParseUlong(),
                Name = name.ToUpperInvariant(),
                State = state.ToUpperInvariant()
            };
        }
    }
}

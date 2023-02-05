// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Billings.Domain.Models;
using Library.Optimizations;

namespace Billings.Domain.Services
{
    /// <inheritdoc cref="IModelFactory"/>
    public class ModelFactory : IModelFactory
    {
        public Billing CreateBilling(string cpfString, double amount, string dueDate)
        {
            return new Billing
            {
                Id = Guid.NewGuid(),
                Cpf = cpfString.AsSpan().ParseUlong(),
                Amount = amount,
                DueDate = new Date(dueDate)
            };
        }
    }
}

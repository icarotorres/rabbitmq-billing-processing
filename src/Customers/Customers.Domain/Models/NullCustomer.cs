// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Library.Results;

namespace Customers.Domain.Models
{
    /// <summary>
    /// Domain representation of a Null Object for business Customer actor
    /// </summary>
    public class NullCustomer : Customer, INull
    {
        /// <inheritdoc cref="Customer.Cpf"/>
        public override ulong Cpf { get => 0; set { _ = value; } }
        public override string Name { get => string.Empty; set { _ = value; } }
        public override string State { get => string.Empty; set { _ = value; } }
    }
}

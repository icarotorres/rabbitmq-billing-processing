// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Customers.Domain.Models
{
    /// <summary>
    /// Domain representation of a business Customer actor
    /// </summary>
    public class Customer
    {
        public static readonly NullCustomer Null = new NullCustomer();
        /// <summary>
        /// Unique personal identification in force in Brazil
        /// </summary>
        public virtual ulong Cpf { get; set; }
        public virtual string Name { get; set; }
        public virtual string State { get; set; }
    }
}

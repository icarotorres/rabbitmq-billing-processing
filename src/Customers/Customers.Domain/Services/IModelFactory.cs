// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Customers.Domain.Models;

namespace Customers.Domain.Services
{
    /// <summary>
    /// Implementation for creating Domain Models
    /// </summary>
    public interface IModelFactory
    {
        /// <summary>
        /// Creates a new complete <see cref="Customer"/> model
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        Customer CreateCustomer(string cpf, string name, string state);
    }
}

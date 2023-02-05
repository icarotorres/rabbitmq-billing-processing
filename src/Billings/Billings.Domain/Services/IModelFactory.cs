// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Billings.Domain.Models;

namespace Billings.Domain.Services
{
    /// <summary>
    /// Implementation for creating Domain Models
    /// </summary>
    public interface IModelFactory
    {
        Billing CreateBilling(string cpfString, double amount, string dueDate);
    }
}

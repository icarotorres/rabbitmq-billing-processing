// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using Customers.Domain.Models;

namespace Customers.Application.Responses
{
    public class CustomerResponse
    {
        public CustomerResponse(Customer customer)
        {
            Cpf = customer.Cpf.ToString().PadLeft(11, '0');
            Name = customer.Name;
            State = customer.State;
        }
        [Required] public string Cpf { get; internal set; }
        [Required] public string Name { get; internal set; }
        [Required] public string State { get; internal set; }
    }
}

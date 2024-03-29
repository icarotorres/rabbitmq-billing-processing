﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using Customers.Application.Responses;
using Library.Requests;

namespace Customers.Application.Requests
{
    public class RegisterCustomerRequest : CreationRequestBase<CustomerResponse>
    {
        [Required] public string Cpf { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string State { get; set; }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using Library.Results;
using MediatR;

namespace Customers.Application.Requests
{
    public class GetCustomerRequest : IRequest<IResult>
    {
        [Required] public string Cpf { get; set; }
    }
}

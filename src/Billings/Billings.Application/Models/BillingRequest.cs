// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using Library.Results;
using MediatR;

namespace Billings.Application.Models
{
    public class BillingRequest : IRequest<IResult>
    {
        [Required] public string Cpf { get; set; }
        [Required] public string DueDate { get; set; }
        [Required] public double Amount { get; set; }
    }
}

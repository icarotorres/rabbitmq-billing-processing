// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Library.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Billings.Application.Models
{
    public class GetBillingsRequest : IRequest<IResult>
    {
        [FromQuery] public string Cpf { get; set; }
        [FromQuery] public string Month { get; set; }
    }
}

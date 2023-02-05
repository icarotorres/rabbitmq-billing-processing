// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Library.Results;
using MediatR;

namespace Processing.Eventual.Domain.Models
{
    public class ProcessedBatch : List<Billing>, IRequest<IResult>
    {
        public ProcessedBatch(IEnumerable<Billing> items) : base(items) { }
    }
}
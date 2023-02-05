// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Library.Results;
using Processing.Eventual.Domain.Services;

namespace Processing.Eventual.Domain.Models
{
    public class NullCustomer : Customer, INull
    {
        public override ulong Cpf { get => 0; set { ; } }
        public override bool AcceptProcessing(Billing billing, IAmountProcessor calculator)
        {
            return false;
        }
    }
}

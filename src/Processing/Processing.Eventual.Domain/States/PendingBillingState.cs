// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Domain.States
{
    public class PendingBillingState : BillingState
    {
        internal PendingBillingState(Billing context) : base(context)
        {
        }
        internal override void BeProcessed(decimal amount, DateTime processedAt)
        {
            Context.ChangeState(new ProcessedBillingState(Context, amount, processedAt));
        }
    }
}

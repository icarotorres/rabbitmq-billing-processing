// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Domain.States
{
    public class ProcessedBillingState : BillingState
    {
        internal ProcessedBillingState(Billing context, decimal amount, DateTime? processedAt) : base(context)
        {
            Amount = amount;
            ProcessedAt = processedAt;
        }

        internal override void BeProcessed(decimal amount, DateTime processedAt)
        {
            // skiping real implementation not applied for this state
        }
    }
}

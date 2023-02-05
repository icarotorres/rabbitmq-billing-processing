// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Domain.States
{
    public abstract class BillingState
    {
        protected BillingState(Billing context)
        {
            Context = context;
        }
        internal static BillingState NewState(Billing context)
        {
            return context.ProcessedAt.HasValue
                ? new ProcessedBillingState(context, context.Amount, context.ProcessedAt)
                : new PendingBillingState(context) as BillingState;
        }
        protected Billing Context { get; set; }
        internal DateTime? ProcessedAt { get; set; }
        internal decimal Amount { get; set; }

        internal abstract void BeProcessed(decimal amount, DateTime processedAt);
    }
}

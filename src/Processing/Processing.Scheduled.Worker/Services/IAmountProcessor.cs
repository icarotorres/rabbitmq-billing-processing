// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Processing.Scheduled.Worker.Models;

namespace Processing.Scheduled.Worker.Services
{
    public interface IAmountProcessor
    {
        Billing Process(ICpfCarrier customer, Billing billing);
    }
}

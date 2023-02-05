// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Processing.Scheduled.Worker.Services;

namespace Processing.Scheduled.Worker.Models
{
    public class Billing : ICpfCarrier
    {
        public Guid Id { get; set; }
        public ulong Cpf { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}

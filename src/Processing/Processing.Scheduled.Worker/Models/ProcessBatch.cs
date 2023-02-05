// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Processing.Scheduled.Worker.Services;

namespace Processing.Scheduled.Worker.Models
{
    public class ProcessBatch
    {
        public ProcessBatch()
        {
            Id = Guid.NewGuid().ToString();
            Billings = new List<Billing>();
            Customers = new List<ICpfCarrier>();
        }
        public string Id { get; set; }
        public List<Billing> Billings { get; set; }
        public List<ICpfCarrier> Customers { get; set; }
    }
}

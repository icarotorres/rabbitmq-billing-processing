// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using Billings.Domain.Models;

namespace Billings.Application.Models
{
    public class BillingResponse
    {
        public BillingResponse(Billing billing)
        {
            Id = billing.Id;
            Amount = billing.Amount;
            Cpf = billing.Cpf.ToString().PadLeft(11, '0');
            DueDate = billing.DueDate.ToString();
        }
        [Required] public Guid Id { get; set; }
        [Required] public string Cpf { get; set; }
        [Required] public string DueDate { get; set; }
        [Required] public double Amount { get; set; }
    }
}

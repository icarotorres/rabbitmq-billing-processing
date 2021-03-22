﻿using BillingIssuance.Api.Domain.Models;
using PrivatePackage.Optmizations;
using System;

namespace BillingIssuance.Api.Domain.Services
{
    public class ModelFactory : IModelFactory
    {
        public Billing CreateBilling(ReadOnlySpan<char> cpfString, double amount, ReadOnlySpan<char> dueDate)
        {
            return new Billing
            {
                Id = Guid.NewGuid(),
                Cpf = cpfString.ParseUlong(),
                Amount = amount,
                DueDate = new Date
                {
                    Day = dueDate.Slice(0, 2).ParseByte(),
                    Month = dueDate.Slice(3, 2).ParseByte(),
                    Year = dueDate.Slice(6, 4).ParseUshort()
                }
            };
        }
    }
}

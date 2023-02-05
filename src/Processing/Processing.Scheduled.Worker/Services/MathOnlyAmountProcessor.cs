﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Processing.Scheduled.Worker.Models;

namespace Processing.Scheduled.Worker.Services
{
    public class MathOnlyAmountProcessor : IAmountProcessor
    {
        private const uint _firstTwoDigitIsolationDivider = 1000000000;
        private const ushort _firstTwoDigitHundredsMultiplier = 100;
        private const ushort _tesnAndUnitsIsolatorMod = 100;

        public Billing Process(ICpfCarrier customer, Billing billing)
        {
            var tensAndUnits = ProcessTensAndUnits(customer.Cpf);
            var thousandsAndhundreds = ProcessThousandsAndHundreds(customer.Cpf);
            billing.Amount = thousandsAndhundreds + tensAndUnits;
            billing.ProcessedAt = DateTime.UtcNow;
            return billing;
        }

        private byte ProcessTensAndUnits(ulong cpf)
        {
            var tensAndUnits = cpf % _tesnAndUnitsIsolatorMod;
            return (byte)tensAndUnits;
        }

        private ushort ProcessThousandsAndHundreds(ulong cpf)
        {
            var firstTwoDigit = IsolateFirstTwoDigit(cpf);
            var thousandsAndhundreds = firstTwoDigit * _firstTwoDigitHundredsMultiplier;
            return (ushort)thousandsAndhundreds;
        }

        private byte IsolateFirstTwoDigit(ulong cpf)
        {
            var firstTwoDigitWithDecimals = cpf / _firstTwoDigitIsolationDivider;
            var firstTwoDigitTruncated = decimal.Truncate(firstTwoDigitWithDecimals);
            return (byte)firstTwoDigitTruncated;
        }
    }
}

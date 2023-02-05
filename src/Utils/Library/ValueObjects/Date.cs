// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Library.Optimizations;
using ValueOf;

namespace Library.ValueObjects
{
    public class Date : ValueOf<string, Date>
    {
        private bool _isValid;
        public bool IsValid() => _isValid;

        protected override void Validate()
        {
            _isValid = TryParse(Value, out _, out _, out _);
        }

        public static bool TryParse(ReadOnlySpan<char> value, out byte day, out byte month, out ushort year)
        {
            day = 0;
            month = 0;
            year = 0;

            return value.Length == 10 &&
                value.Slice(6, 4).TryParseUshort(out year) &&
                value.Slice(3, 2).TryParseByte(out month) &&
                value[..2].TryParseByte(out day);
        }

        public static bool ValidateFutureDate(ReadOnlySpan<char> duedate)
        {
            return TryParse(duedate, out var day, out var month, out var year) &&
            (
                // future year
                (year > DateTime.Today.Year) ||
                // future month
                (year == DateTime.Today.Year && month > DateTime.Today.Month) ||
                // future day
                (year == DateTime.Today.Year && month == DateTime.Today.Month && day > DateTime.Today.Day)
            );
        }

        public static bool TryParseMonth(ReadOnlySpan<char> monthYear, out byte month, out ushort year)
        {
            month = 0;
            year = 0;
            return monthYear.Length == 7
&& monthYear[..2].TryParseByte(out month) && month >= 1 && month <= 12
&& monthYear.Slice(3, 4).TryParseUshort(out year) && year >= 2000;
        }
    }
}

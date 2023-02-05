// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Library.Optimizations;
using ValueOf;

namespace Library.ValueObjects
{
    public class Cpf : ValueOf<ulong, Cpf>
    {
        private const int MIN_RANDOM_NUMBER_VALUE = 1000000;
        private const int MAX_RANDOM_NUMBER_VALUE = 900000000;
        private const string ZEROS = "00000000000";
        private const string NINES = "99999999999";
        private static readonly Random _random = new Random(123);
        private static readonly object _syncObj = new object();
        private bool _isValid;
        public bool IsValid() => _isValid;

        public static Cpf NewCpf()
        {
            Cpf cpf;
            do
            {
                cpf = From(GenerateRandom());
            }
            while (!cpf.IsValid());
            return cpf;
        }

        public static bool TryParse(string value, out ulong parsedValue)
        {
            ReadOnlySpan<char> cpf = value ?? "0";
            return cpf.TryParseUlong(out parsedValue);
        }

        public static ulong GenerateRandom()
        {
            lock (_syncObj)
            {
                var number = (ulong)_random.Next(MIN_RANDOM_NUMBER_VALUE, MAX_RANDOM_NUMBER_VALUE);
                var seed = number.ToString("000000000");
                var digit1 = GenerateVerifierDigit1(seed);
                var digit2 = GenerateVerifierDigit2(seed, digit1);
                var numberToSumDigits = number * 100;
                var digit1AsTens = (ulong)(digit1 * 10);
                var numberAndDigit1 = numberToSumDigits + digit1AsTens;
                var numberFinishedByDigits = numberAndDigit1 + digit2;
                return numberFinishedByDigits;
            }
        }

        public override string ToString()
        {
            return Value.ToString("00000000000");
        }

        public static bool Validate(ReadOnlySpan<char> value)
        {
            var str = value.ToString();
            if (!ValidateFormat(value) || str == ZEROS || str == NINES)
            {
                return false;
            }

            var seed = value[..9];
            var verifierDigits = value.Slice(9, 2);
            var verifierDigit1 = GenerateVerifierDigit1(seed);
            if (verifierDigits[0] - '0' != verifierDigit1)
            {
                return false;
            }

            var verifierDigit2 = GenerateVerifierDigit2(seed, verifierDigit1);
            return verifierDigits[1] - '0' == verifierDigit2;
        }

        protected override void Validate()
        {
            _isValid = Validate(Value.ToString());
        }

        private static bool ValidateFormat(ReadOnlySpan<char> value)
        {
            if (value.Length != 11)
            {
                return false;
            }

            foreach (var c in value)
            {
                if (!char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        private static byte GenerateVerifierDigit1(ReadOnlySpan<char> seed)
        {
            var multiplier1 = new byte[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = SumIteratedMultipliers(multiplier1, seed);
            return Mod11(sum);
        }

        private static byte GenerateVerifierDigit2(ReadOnlySpan<char> seed, byte verifierDigit1)
        {
            var multiplier2 = new byte[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = SumIteratedMultipliers(multiplier2, seed);
            sum += verifierDigit1 * multiplier2[^1];
            return Mod11(sum);
        }

        private static int SumIteratedMultipliers(byte[] multiplier, ReadOnlySpan<char> seed)
        {
            var sum = 0;
            for (byte i = 0; i < 9; i++)
            {
                sum += (seed[i] - '0') * multiplier[i];
            }

            return sum;
        }

        private static byte Mod11(int sum)
        {
            var verifierDigit2 = (byte)(sum % 11);
            return (byte)(verifierDigit2 < 2 ? 0 : 11 - verifierDigit2);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using ValueOf;

namespace Library.ValueObjects
{
    public class BrazilianState : ValueOf<string, BrazilianState>
    {
        private bool _isValid;
        public bool IsValid() => _isValid;
        protected override void Validate() => _isValid = Validate(Value);
        public static bool Validate(string value) => _states.Contains(value);

        private static readonly HashSet<string> _states = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
            "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
            "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };
    }
}

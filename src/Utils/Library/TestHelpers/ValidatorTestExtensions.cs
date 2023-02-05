// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using FluentValidation.Results;

namespace Library.TestHelpers
{
    public static class ValidatorTestExtensions
    {
        public static void AssertValidationFailuresCount(this ValidationResult result, int expectedErrorsCount)
        {
            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(expectedErrorsCount);
        }
    }
}

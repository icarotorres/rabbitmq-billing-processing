﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Library.Results
{

    public static class MessageExtensions
    {
        public static IEnumerable<string> ExtractMessages(this Exception ex)
        {
            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return messages;
        }

        public static IEnumerable<string> ExtractMessages(this IEnumerable<ValidationFailure> validationFailures)
        {
            return validationFailures is null
                ? new string[] { }
                : validationFailures.Select(x => x.ErrorMessage);
        }
    }
}

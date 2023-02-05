// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Results
{
    /// <summary>
    /// A Failed execution result holding error messages
    /// </summary>

    public class FailResult : OkObjectResult, IResult
    {
        public FailResult(int status, IEnumerable<string> errors) : base(default)
        {
            StatusCode = status;
            _errors = errors.ToArray();
            Value = new { Data = (object)null, Errors = errors };
        }
        public FailResult(IEnumerable<ValidationFailure> failures) : base(default)
        {
            _errors = failures.ExtractMessages().ToArray();
            StatusCode = int.TryParse(failures.FirstOrDefault()?.ErrorCode, out var statusFromValidation)
                ? statusFromValidation
                : StatusCodes.Status400BadRequest;
            Value = new { Data = (object)null, Errors = _errors };
        }

        private readonly string[] _errors;

        public bool IsSuccess() => false;
        public int GetStatus() => StatusCode ?? StatusCodes.Status409Conflict;
        public object GetData() => default;
        public IReadOnlyList<string> Errors => _errors;
    }
}

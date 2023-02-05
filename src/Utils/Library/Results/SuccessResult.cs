// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Results
{
    /// <summary>
    /// General execution success result unable to carry errors and used on happy paths
    /// </summary>

    public class SuccessResult : OkObjectResult, IResult
    {
        public SuccessResult(object data, int status = StatusCodes.Status200OK) : base(default)
        {
            _data = data;
            _errors = new string[] { };
            StatusCode = status;
            Value = new { Data = data, Errors = _errors };
        }

        private readonly object _data;
        private readonly string[] _errors;

        public bool IsSuccess() => true;
        public int GetStatus() => StatusCode ?? StatusCodes.Status200OK;
        public object GetData() => _data;
        public IReadOnlyList<string> Errors => _errors;
    }
}

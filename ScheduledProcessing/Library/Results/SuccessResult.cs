﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Library.Results
{
    [ExcludeFromCodeCoverage]
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

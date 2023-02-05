// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace Library.Results
{
    /// <summary>
    /// Class expressing default api output contract to swagger docs.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public abstract class Output<T>
    {
        public T Data { get; set; }
        [Required] public string[] Errors { get; set; }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace Library.Caching
{
    public interface ICacheService
    {
        Task<string> GetAsync(string key);
        Task<bool> SetAsync(string key, string value, int timeInSeconds);
    }
}

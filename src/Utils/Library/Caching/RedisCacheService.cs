// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Library.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<string> GetAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task<bool> SetAsync(string key, string value, int timeInSeconds)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var expiry = TimeSpan.FromSeconds(timeInSeconds);
            return await db.StringSetAsync(key, value, expiry);
        }
    }
}

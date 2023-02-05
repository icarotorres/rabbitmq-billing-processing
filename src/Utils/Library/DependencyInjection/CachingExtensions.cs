// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Library.Caching;
using Library.Configurations;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CachingExtensions
    {
        public static IServiceCollection BootstrapCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redis = configuration.GetSection("Redis").Get<RedisSettings>();
            services
                .AddSingleton(_ => redis)
                .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redis.ConnectionString))
                .AddSingleton<ICacheService, RedisCacheService>();

            return services;
        }
    }
}

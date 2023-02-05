// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Billings.Application.Abstractions;
using Billings.Infrastructure.Persistence;
using Billings.Infrastructure.Persistence.Services;
using Library.Configurations;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection BootstrapPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDB = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
            return services
                .AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(mongoDB.ConnectionString))
                .AddSingleton(x => x.GetRequiredService<IMongoClient>().GetDatabase(mongoDB.DatabaseName))
                .AddSingleton(mongoDB.Collections)
                .AddSingleton<IBillingsContext, BillingsContext>()
                .AddSingleton<IBillingRepository, BillingRepository>();
        }
    }
}

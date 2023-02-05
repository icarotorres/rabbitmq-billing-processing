// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Library.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Processing.Eventual.Application.Abstractions;
using Processing.Eventual.Worker.Persistence;
using Processing.Eventual.Worker.Persistence.Services;

namespace Processing.Eventual.Worker.DependencyInjection
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection BootstrapPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            var mongoConnectionString = config["MongoDB:ConnectionString"];
            var database = config["MongoDB:DatabaseName"];
            var collections = config.GetSection("MongoDB:Collections").Get<CollectionsDictionary>();

            return services
                .AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(mongoConnectionString))
                .AddSingleton(x => x.GetRequiredService<IMongoClient>().GetDatabase(database))
                .AddSingleton(collections)
                .AddSingleton<IBillingProcessingContext, BillingProcessingContext>()
                .AddSingleton<ICustomerRepository, CustomerRepository>()
                .AddSingleton<IBillingsRepository, BillingsRepository>();
        }
    }
}

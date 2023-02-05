// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Customers.Application.Abstractions;
using Customers.Application.Workers;
using Customers.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class PersistenceExtensions
    {
        public static IServiceCollection BootstrapPersistenceServices(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            return services
                .AddHostedService<ScheduledCustomerAcceptProcessWorker>()
                .AddDbContext<CustomersContext>(options =>
                {
                    var sourcePath = Path.Combine(env.ContentRootPath, ".\\Infrastructure\\Persistence", configuration["SQLite:DatabaseName"]);
                    options.UseSqlite($"Data Source={sourcePath}");
                    if (!env.IsProduction())
                    {
                        options.EnableSensitiveDataLogging();
                    }
                })
                .AddScoped<DbContext, CustomersContext>()
                .AddSingleton<ICustomerRepositoryFactory>(x =>
                {
                    var dbOptions = x.CreateScope()
                        .ServiceProvider
                        .GetRequiredService<DbContextOptions<CustomersContext>>();
                    return new CustomerRepositoryFactory(dbOptions);
                })
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IUnitofwork, Unitofwork>();
        }
    }
}

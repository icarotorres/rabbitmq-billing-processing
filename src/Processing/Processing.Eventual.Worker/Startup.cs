// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Processing.Eventual.Application.Workers;
using Processing.Eventual.Worker.DependencyInjection;

namespace Processing.Eventual.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor()
                .BootstrapPersistenceServices(Configuration)
                .BootstrapMessagingServices(Configuration)
                .BootstrapPipelinesServices()
                .AddHostedService<ConsumersBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthAllEndpoints();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Compression;
using Library.Configurations;
using Library.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Billings.Api
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
                .BootstrapDomainServices()
                .BootstrapValidators()
                .BootstrapPersistenceServices(Configuration)
                .BootstrapCache(Configuration)
                .BootstrapSwaggerConfig(Configuration)
                .BootstrapWorkerServices(Configuration)
                .BootstrapMessagingServices(Configuration)
                .BootstrapPipelinesServices()
                .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>())
                .AddControllers()
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SwaggerSettings swagger)
        {
            app = env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseHsts();

            app.UseMiddleware(typeof(ExceptionMiddleware));

            app.UseSwagger(option => option.RouteTemplate = swagger.Template)
               .UseSwaggerUI(option => option.SwaggerEndpoint(swagger.Url, swagger.Title));

            app.UseHttpsRedirection()
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(e => e.MapControllers());
        }
    }
}

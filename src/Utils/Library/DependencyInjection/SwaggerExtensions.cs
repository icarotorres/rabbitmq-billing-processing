// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using Library.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection BootstrapSwaggerConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var swagger = configuration.GetSection("Swagger").Get<SwaggerSettings>();
            services.AddSingleton(_ => swagger);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swagger.Version, new OpenApiInfo
                {
                    Title = swagger.Title,
                    Description = swagger.Description,
                    Version = swagger.Version
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            }).AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }
}

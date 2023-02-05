// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FluentValidation;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class FluentValidationsExtensions
    {
        public static IServiceCollection BootstrapValidators(this IServiceCollection services)
        {
            return services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

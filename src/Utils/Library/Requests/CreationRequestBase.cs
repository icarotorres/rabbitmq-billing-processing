// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Mvc;

namespace Library.Requests
{
    /// <inheritdoc cref="ICreationRequest{T}"/>

    public abstract class CreationRequestBase<T> : ICreationRequest<T>
    {
        protected string Route { get; set; }
        protected IUrlHelper UrlHelper { get; set; }
        protected Func<T, object> RouteValuesFunc { get; set; }

        public string GetRouteName() => Route;
        public IUrlHelper GetUrlHelper() => UrlHelper;
        public Func<T, object> GetRouteValuesFunc() => RouteValuesFunc;

        public ICreationRequest<T> SetupForCreation(IUrlHelper urlHelper, string route, Func<T, object> routeValuesFunc)
        {
            UrlHelper = urlHelper;
            Route = route;
            RouteValuesFunc = routeValuesFunc;
            return this;
        }
    }
}

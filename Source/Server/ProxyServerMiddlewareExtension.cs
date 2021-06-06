// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;

namespace Dolittle.Runtime.Server
{
    public static class ProxyServerMiddlewareExtension
    {
        /// <summary>
        /// Adds a <see cref="ProxyServerMiddleware"/> to have a route that gets proxied through to a target URL.
        /// </summary>
        /// <param name="builder"><see cref="IApplicationBuilder"/> to add middleware to.</param>
        /// <param name="route">The relative - rooted route (e.g. /proxy).</param>
        /// <param name="target">The fully qualified URL to proxy through to.</param>
        /// <returns><see cref="IApplicationBuilder"/> for building further.</returns>
        public static IApplicationBuilder UseProxyServer(this IApplicationBuilder builder, string route, string target)
        {
            return builder.Use(next => new ProxyServerMiddleware(route, target, next).Invoke);
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;

namespace Dolittle.Runtime.Server
{
    public static class ProxyServerMiddlewareExtension
    {
        public static IApplicationBuilder UseProxyServer(this IApplicationBuilder builder, string route, string target)
        {
            return builder.Use(next => new ProxyServerMiddleware(route, target, next).Invoke);
        }
    }
}

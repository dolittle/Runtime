// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Server.Grpc;

public static class Servers
{
    public static IHostBuilder ConfigureGrpcServers(this IHostBuilder builder)
    {
        // For each exposed server (public, private, management)
        builder.ConfigureWebHost(_ =>
        {
            _.UseKestrel((context, _) =>
            {
                var config = context.Configuration.GetSection("public:port");
                var port = config.Value;
                var visibility = config.Key;

                if (visibility == "private")
                {
                    
                }
                
                var children = config.GetChildren();
            });
            _.Configure((context, _) => _.UseEndpoints(endpoints =>
            {
                // endpoints.MapGrpcService<>();
            }));
        });
        return builder;
    }
}

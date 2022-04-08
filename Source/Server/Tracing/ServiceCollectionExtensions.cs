// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Proto.OpenTelemetry;

namespace Dolittle.Runtime.Server.Tracing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTracing(this IServiceCollection services)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("dolittle-runtime"))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddGrpcClientInstrumentation()
                .AddProtoActorInstrumentation()
                .AddJaegerExporter(options => options.AgentHost = "localhost");
            // .AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:8200")); // TODO: replace with config
        });
        return services;
    }
}

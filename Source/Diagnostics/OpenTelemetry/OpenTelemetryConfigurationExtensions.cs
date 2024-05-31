// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Proto.OpenTelemetry;

namespace Dolittle.Runtime.Diagnostics.OpenTelemetry;

public static class OpenTelemetryConfigurationExtensions
{
    public static IHostBuilder ConfigureOpenTelemetry(this IHostBuilder builder, IConfiguration cfg)
    {
        var configuration = cfg.GetSection("dolittle:runtime:opentelemetry").Get<OpenTelemetryConfiguration>();

        if (configuration?.Endpoint is null)
        {
            return builder;
        }

        if (!Uri.TryCreate(configuration.Endpoint, UriKind.RelativeOrAbsolute, out var otlpEndpoint))
        {
            var logger = LoggerFactory.Create(opt => opt.AddConfiguration(cfg))
                .CreateLogger(typeof(OpenTelemetryConfigurationExtensions));
#pragma warning disable CA1848
            logger.LogWarning("Unable to parse otlp endpoint {Input}", configuration.Endpoint);
#pragma warning restore CA1848
            return builder;
        }

        if (otlpEndpoint.Scheme.Equals("http"))
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(configuration.ServiceName);

        if (configuration.Logging)
        {
            builder.AddOpenTelemetryLogging(resourceBuilder, otlpEndpoint);
        }

        if (configuration.Tracing)
        {
            builder.AddOpenTelemetryTracing(resourceBuilder, otlpEndpoint);
        }

        if (configuration.Metrics)
        {
            builder.AddOpenTelemetryMetrics(resourceBuilder, otlpEndpoint);
        }

        return builder;
    }

    static void AddOpenTelemetryLogging(this IHostBuilder builder, ResourceBuilder resourceBuilder, Uri otlpEndpoint)
    {
        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder
                // Span & Trace ID's are published by OTLP without adding them to scope
                .Configure(options => options.ActivityTrackingOptions = ActivityTrackingOptions.None)
                .AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.IncludeFormattedMessage = true;
                    options.SetResourceBuilder(resourceBuilder)
                        .AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = otlpEndpoint);
                });
        });
    }

    static void AddOpenTelemetryTracing(this IHostBuilder builder, ResourceBuilder resourceBuilder, Uri otlpEndpoint)
    {
        builder.ConfigureServices(services =>
            services.AddOpenTelemetry()
                .WithTracing(providerBuilder =>
                {
                    providerBuilder.SetResourceBuilder(resourceBuilder)
                        .AddSource(RuntimeActivity.SourceName)
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddMongoDBInstrumentation()
                        .AddGrpcClientInstrumentation()
                        .AddProtoActorInstrumentation()
                        .AddOtlpExporter(options => { options.Endpoint = otlpEndpoint; });
                }));
    }

    static void AddOpenTelemetryMetrics(this IHostBuilder builder, ResourceBuilder resourceBuilder, Uri otlpEndpoint)
    {
        builder.ConfigureServices(services =>
            services.AddOpenTelemetry()
                .WithMetrics(providerBuilder =>
                {
                    providerBuilder.SetResourceBuilder(resourceBuilder)
                        .AddMeter(RuntimeMetrics.SourceName)
                        .AddAspNetCoreInstrumentation()
                        .AddProtoActorInstrumentation()
                        .AddOtlpExporter(options => { options.Endpoint = otlpEndpoint; });
                }));
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Reflection;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Options;
using Services.Clients;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="ICallInvokerManager"/>.
/// </summary>
[Singleton]
public class CallInvokerManager : ICallInvokerManager
{
    readonly IKnownClients _knownClients;
    readonly ClientEndpointsConfiguration _configuration;
    readonly IMetadataProviders _metadataProviders;

    /// <summary>
    /// Initializes a new instance of the <see cref="CallInvokerManager"/> class.
    /// </summary>
    /// <param name="knownClients">All the <see cref="IKnownClients"/>.</param>
    /// <param name="configuration"><see cref="ClientEndpointsConfiguration">Configuration</see>.</param>
    /// <param name="metadataProviders"><see cref="IMetadataProviders"/> for providing metadata to calls.</param>
    public CallInvokerManager(
        IKnownClients knownClients,
        IOptions<ClientEndpointsConfiguration> configuration,
        IMetadataProviders metadataProviders)
    {
        _knownClients = knownClients;
        _configuration = configuration.Value;
        _metadataProviders = metadataProviders;
    }

    /// <inheritdoc/>
    public CallInvoker GetFor(Type type, string host = default, int port = default)
    {
        ThrowIfTypeDoesNotImplementClientBase(type);

        var client = _knownClients.GetFor(type);
        var endpointConfiguration = _configuration[client.Visibility];

        var keepAliveTime = new ChannelOption("grpc.keepalive_time", 1000);
        var keepAliveTimeout = new ChannelOption("grpc.keepalive_timeout_ms", 500);
        var keepAliveWithoutCalls = new ChannelOption("grpc.keepalive_permit_without_calls", 1);
        host = host == default ? endpointConfiguration.Host : host;
        port = port == default ? endpointConfiguration.Port : port;
        var channel = new Channel(
            host,
            port,
            ChannelCredentials.Insecure,
            new[] { keepAliveTime, keepAliveTimeout, keepAliveWithoutCalls });

        return channel.Intercept(_ =>
        {
            _metadataProviders.Provide().ForEach(_.Add);
            return _;
        });
    }

    void ThrowIfTypeDoesNotImplementClientBase(Type type)
    {
        if (!type.Implements(typeof(ClientBase)))
        {
            throw new TypeDoesNotImplementClientBase(type);
        }
    }
}

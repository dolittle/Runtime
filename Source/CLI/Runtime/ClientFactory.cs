// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Services.Clients;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Dolittle.Runtime.CLI.Runtime;

/// <summary>
/// Represents an implementation of <see cref="ICanCreateClients"/>.
/// </summary>
public class ClientFactory : ICanCreateClients
{
    /// <inheritdoc />
    public T CreateClientFor<T>(MicroserviceAddress address)
        where T : ClientBase<T>
    {
        var constructor = typeof(T).GetConstructor(new[] {typeof(CallInvoker)});
        ThrowIfMissingExpectedConstructorClientType(typeof(T), constructor);

        var callInvoker = CreateCallInvoker(address);
        return constructor!.Invoke(new object[] {callInvoker}) as T;
    }

    static CallInvoker CreateCallInvoker(MicroserviceAddress address)
    {
        var keepAliveTime = new ChannelOption("grpc.keepalive_time", 1000);
        var keepAliveTimeout = new ChannelOption("grpc.keepalive_timeout_ms", 500);
        var keepAliveWithoutCalls = new ChannelOption("grpc.keepalive_permit_without_calls", 1);

        var channel = new Channel(
            address.Host,
            address.Port,
            ChannelCredentials.Insecure,
            new[] {keepAliveTime, keepAliveTimeout, keepAliveWithoutCalls});

        return channel.Intercept(_ => _);
    }

    static void ThrowIfMissingExpectedConstructorClientType(Type type, ConstructorInfo constructor)
    {
        if (constructor == null)
        {
            throw new MissingExpectedConstructorForClientType(type);
        }
    }
}
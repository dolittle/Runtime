// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Runtime.Services.Clients;
using Grpc.Core;
using Grpc.Net.Client;
using Microservices;

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
        var constructor = typeof(T).GetConstructor(new[] {typeof(ChannelBase)});
        ThrowIfMissingExpectedConstructorClientType(typeof(T), constructor);
        return constructor!.Invoke(new object[] {GrpcChannel.ForAddress($"http://{address.Host}:{address.Port}")}) as T;
    }

    static void ThrowIfMissingExpectedConstructorClientType(Type type, ConstructorInfo constructor)
    {
        if (constructor == null)
        {
            throw new MissingExpectedConstructorForClientType(type);
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="IClientManager"/>.
/// </summary>
public class ClientManager : IClientManager
{
    readonly IChannels _channels;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientManager"/> class.
    /// </summary>
    /// <param name="channels"><see cref="IChannels"/> to get <see cref="CallInvoker"/> from.</param>
    public ClientManager(IChannels channels)
    {
        _channels = channels;
    }

    /// <inheritdoc/>
    public ClientBase Get(Type type, string host, int port)
    {
        ThrowIfTypeDoesNotImplementClientBase(type);
        var constructor = type.GetConstructor(new[] { typeof(ChannelBase) });
        ThrowIfMissingExpectedConstructorClientType(type, constructor);

        return constructor.Invoke(new[] { _channels.GetFor(host, port) }) as ClientBase;
    }

    /// <inheritdoc/>
    public TClient Get<TClient>(string host, int port)
        where TClient : ClientBase => Get(typeof(TClient), host, port) as TClient;

    static void ThrowIfTypeDoesNotImplementClientBase(Type type)
    {
        if (!typeof(ClientBase).IsAssignableFrom(type))
        {
            throw new TypeDoesNotImplementClientBase(type);
        }
    }

    static void ThrowIfMissingExpectedConstructorClientType(Type type, ConstructorInfo constructor)
    {
        if (constructor == null)
        {
            throw new MissingExpectedConstructorForClientType(type);
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Reflection;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="IKnownClients"/>.
/// </summary>
public class KnownClients : IKnownClients
{
    readonly Dictionary<Type, Client> _clientsByType = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownClients"/> class.
    /// </summary>
    /// <param name="clientProviders"><see cref="IEnumerable{T}"/> <see cref="IKnowAboutClients"/>.</param>
    public KnownClients(IEnumerable<IKnowAboutClients> clientProviders)
    {
        clientProviders.ForEach(provider => provider.Clients.ForEach(client => _clientsByType.Add(client.Type, client)));
    }

    /// <inheritdoc/>
    public Client GetFor(Type type)
    {
        ThrowIfTypeDoesNotImplementClientBase(type);
        ThrowIfUnknownClientType(type);
        return _clientsByType[type];
    }

    /// <inheritdoc/>
    public bool HasFor(Type type)
    {
        ThrowIfTypeDoesNotImplementClientBase(type);
        return _clientsByType.ContainsKey(type);
    }

    void ThrowIfTypeDoesNotImplementClientBase(Type type)
    {
        if (!type.Implements(typeof(ClientBase)))
        {
            throw new TypeDoesNotImplementClientBase(type);
        }
    }

    void ThrowIfUnknownClientType(Type type)
    {
        if (!_clientsByType.ContainsKey(type))
        {
            throw new UnknownClientType(type);
        }
    }
}

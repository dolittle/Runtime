// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Defines a manager of <see cref="ClientBase">clients</see>.
/// </summary>
public interface IClientManager
{
    /// <summary>
    /// Get a specific type of <see cref="ClientBase"/>.
    /// </summary>
    /// <param name="type">Type of <see cref="ClientBase"/> to get.</param>
    /// <param name="host">The host the client should connect to. If default value it uses the 'clients' configuration.</param>
    /// <param name="port">The port the client should connect on. If default value it uses the 'clients' configuration.</param>
    /// <returns><see cref="ClientBase"/> instance.</returns>
    ClientBase Get(Type type, string host, int port);

    /// <summary>
    /// /// Get a specific type of <see cref="ClientBase"/>.
    /// </summary>
    /// <param name="host">The host the client should connect to. If default value it uses the 'clients' configuration.</param>
    /// <param name="port">The port the client should connect on. If default value it uses the 'clients' configuration.</param>
    /// <typeparam name="TClient">The type of client.</typeparam>
    /// <returns>The <see cref="ClientBase" /> instance.</returns>
    TClient Get<TClient>(string host, int port)
        where TClient : ClientBase;
}

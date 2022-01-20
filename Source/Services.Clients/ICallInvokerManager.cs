// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Defines a system that manages <see cref="CallInvoker">call invokers</see>.
/// </summary>
public interface ICallInvokerManager
{
    /// <summary>
    /// Get for a specific type of client - must implement <see cref="ClientBase"/>.
    /// </summary>
    /// <param name="type">Type of client to get for - must implement <see cref="ClientBase"/>.</param>
    /// <param name="host">The host the client should connect to. If default value it uses the 'clients' configuration.</param>
    /// <param name="port">The port the client should connect on. If default value it uses the 'clients' configuration.</param>
    /// <returns><see cref="CallInvoker"/>.</returns>
    CallInvoker GetFor(Type type, string host = default, int port = default);
}
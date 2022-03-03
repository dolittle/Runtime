// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;
using Grpc.Net.Client;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Defines a system knows about <see cref="Grpc.Net.Client.GrpcChannel"/>.
/// </summary>
public interface IChannels
{
    /// <summary>
    /// Get for a an address
    /// </summary>
    /// <param name="host">The host the client should connect to.</param>
    /// <param name="port">The port the client should connect on.</param>
    /// <returns><see cref="CallInvoker"/>.</returns>
    GrpcChannel GetFor(string host, int port);
}

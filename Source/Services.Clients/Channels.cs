// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Grpc.Net.Client;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="IChannels"/>.
/// </summary>
[Singleton]
public class Channels : IChannels
{
    // readonly ConcurrentDictionary<Uri>
    
    //TODO: This should create one channel per address and keep it alive.
    /// <inheritdoc/>
    public GrpcChannel GetFor(string host, int port) => GrpcChannel.ForAddress($"http://{host}:{port}");
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Microservices;
using Grpc.Core;

namespace Dolittle.Runtime.CLI.Runtime
{
    /// <summary>
    /// Defines a system that can create gRPC clients.
    /// </summary>
    public interface ICanCreateClients
    {
        /// <summary>
        /// Creates a client for the specified type that connects to the provided Runtime address.
        /// </summary>
        /// <param name="address">The address to use to connect to the Runtime.</param>
        /// <typeparam name="T">The type of the client to create.</typeparam>
        /// <returns>A new <typeparamref name="T"/>.</returns>
        T CreateClientFor<T>(MicroserviceAddress address)
            where T : ClientBase<T>;
    }
}
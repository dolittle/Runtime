// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a system that knows about all clients.
    /// </summary>
    public interface IKnownClients
    {
        /// <summary>
        /// Check if there is a <see cref="Client"/> definition for a specific type of <see cref="ClientBase">client</see>.
        /// </summary>
        /// <param name="type">Type of <see cref="ClientBase"/> to check for.</param>
        /// <returns>true if there is a definition, false if not.</returns>
        bool HasFor(Type type);

        /// <summary>
        /// Get for a specific type of <see cref="ClientBase">client</see>.
        /// </summary>
        /// <param name="type">Type of <see cref="ClientBase"/> to get for.</param>
        /// <returns><see cref="Client"/> instance.</returns>
        Client GetFor(Type type);
    }
}
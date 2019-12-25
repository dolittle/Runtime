// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Defines a system for working with connected clients.
    /// </summary>
    public interface IConnectedHeads
    {
        /// <summary>
        /// Connect a client.
        /// </summary>
        /// <param name="client"><see cref="Head"/> to connect.</param>
        void Connect(Head client);

        /// <summary>
        /// Disconnect a client by its <see cref="HeadId"/>.
        /// </summary>
        /// <param name="clientId"><see cref="HeadId"/> to disconnect.</param>
        void Disconnect(HeadId clientId);

        /// <summary>
        /// Check if a client by its <see cref="HeadId"/> is connected.
        /// </summary>
        /// <param name="clientId"><see cref="HeadId"/> of <see cref="Head"/> to check.</param>
        /// <returns>True if is connected, false if not.</returns>
        bool IsConnected(HeadId clientId);

        /// <summary>
        /// Get all connected clients.
        /// </summary>
        /// <returns>Collection of <see cref="Head">clients</see>.</returns>
        IEnumerable<Head> GetAll();

        /// <summary>
        /// Get the client that exposes the <see cref="ClientBase"/>.
        /// </summary>
        /// <typeparam name="TC">Type of <see cref="ClientBase"/>.</typeparam>
        /// <returns>Currently connected <see cref="Head"/>.</returns>
        Head GetFor<TC>()
            where TC : ClientBase;

        /// <summary>
        /// Get the client that exposes the <see cref="ClientBase"/>.
        /// </summary>
        /// <param name="type">Type of <see cref="ClientBase"/>.</param>
        /// <returns>Currently connected <see cref="Head"/>.</returns>
        Head GetFor(Type type);

        /// <summary>
        /// Get a client based on its <see cref="HeadId"/>.
        /// </summary>
        /// <param name="clientId"><see cref="HeadId"/> to get for.</param>
        /// <returns><see cref="Head"/> instance.</returns>
        Head GetById(HeadId clientId);
    }
}
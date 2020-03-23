// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Defines a system for working with connected clients.
    /// </summary>
    public interface IConnectedHeads
    {
        /// <summary>
        /// Gets an <see cref="ObservableCollection{T}"/> for all heads.
        /// </summary>
        ObservableCollection<Head> All { get; }

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
        /// Get a client based on its <see cref="HeadId"/>.
        /// </summary>
        /// <param name="clientId"><see cref="HeadId"/> to get for.</param>
        /// <returns><see cref="Head"/> instance.</returns>
        Head GetById(HeadId clientId);
    }
}
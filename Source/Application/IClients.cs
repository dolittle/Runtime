/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Defines a system for working with connected clients
    /// </summary>
    public interface IClients
    {
        /// <summary>
        /// Connect a client
        /// </summary>
        /// <param name="client"><see cref="Client"/> to connect</param>
        void Connect(Client client);

        /// <summary>
        /// Disconnect a client by its <see cref="ClientId"/>
        /// </summary>
        /// <param name="clientId"><see cref="ClientId"/> to disconnect</param>
        void Disconnect(ClientId clientId);

        /// <summary>
        /// Check if a client by its <see cref="ClientId"/> is connected
        /// </summary>
        /// <param name="clientId"><see cref="ClientId"/> of <see cref="Client"/> to check</param>
        /// <returns>True if is connected, false if not</returns>
        bool IsConnected(ClientId clientId);

        /// <summary>
        /// Get all connected clients
        /// </summary>
        /// <returns>Collection of <see cref="Client">clients</see></returns>
        IEnumerable<Client> GetConnectedClients();

        /// <summary>
        /// Get the client that exposes the <see cref="ClientBase"/>
        /// </summary>
        /// <typeparam name="TC">Type of <see cref="ClientBase"/></typeparam>
        /// <returns>Currently connected <see cref="Client"/></returns>
        Client GetFor<TC>() where TC:ClientBase;

        /// <summary>
        /// Get the client that exposes the <see cref="ClientBase"/>
        /// </summary>
        /// <param name="type">Type of <see cref="ClientBase"/></param>
        /// <returns>Currently connected <see cref="Client"/></returns>
        Client GetFor(Type type);

    }
}
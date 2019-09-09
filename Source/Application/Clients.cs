/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Lifecycle;
using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents an implementation of <see cref="IClients"/>
    /// </summary>
    [Singleton]
    public class Clients : IClients
    {
        readonly List<Client> _clients = new List<Client>();


        /// <inheritdoc/>
        public void Connect(Client client)
        {
            lock( _clients ) _clients.Add(client);
        }

        /// <inheritdoc/>
        public void Disconnect(ClientId clientId)
        {
            lock( _clients ) 
            {
                var client = _clients.SingleOrDefault(_ => _.ClientId == clientId);
                _clients?.Remove(client);
            }
        }

        /// <inheritdoc/>
        public bool IsConnected(ClientId clientId)
        {
            lock( _clients ) 
            {
                return _clients.Any(_ => _.ClientId == clientId);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Client> GetConnectedClients()
        {
            lock( _clients )
            {
                var clients = _clients.ToArray();
                return clients;
            }
        }

        /// <inheritdoc/>
        public Client GetFor<TC>() where TC:ClientBase
        {
            return _clients.Last();
        }

        /// <inheritdoc/>
        public Client GetFor(Type type)
        {
            return _clients.Last();
        }
    }
}
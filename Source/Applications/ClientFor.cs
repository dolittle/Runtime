/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Lifecycle;
using Grpc.Core;

namespace Dolittle.Runtime.Applications
{
    /// <summary>
    /// Represents an implementation of <see cref="IClientFor{T}"/>
    /// </summary>
    [Singleton]
    public class ClientFor<T> : IClientFor<T> where T : ClientBase
    {
        readonly IClients _clients;
        T _instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clients"></param>
        public ClientFor(IClients clients)
        {
            _clients = clients;
        }

        /// <inheritdoc/>
        public T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var type = typeof(T);
                    var client = _clients.GetFor(type);
                    client.Disconnected += (c) => _instance = null;

                    var constructor = type.GetConstructor(new Type[]  {  typeof(ChannelBase) });
                    _instance = constructor.Invoke(new [] { client.Channel }) as T;
                }
                return _instance;
            }
        }
    }
}
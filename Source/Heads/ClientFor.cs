/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Lifecycle;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents an implementation of <see cref="IClientFor{T}"/>
    /// </summary>
    [Singleton]
    public class ClientFor<T> : IClientFor<T> where T : ClientBase
    {
        readonly IConnectedHeads _connectedHeads;
        T _instance;

        /// <summary>
        /// Initializes a new instance of <see cref="ClientFor{T}"/>
        /// </summary>
        /// <param name="connectedHeads"><see cref="IConnectedHeads"/> to use for maintaining connection state</param>
        public ClientFor(IConnectedHeads connectedHeads)
        {
            _connectedHeads = connectedHeads;
        }

        /// <inheritdoc/>
        public T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var type = typeof(T);
                    var client = _connectedHeads.GetFor(type);
                    client.Disconnected += (c) => _instance = null;

                    var constructor = type.GetConstructor(new Type[]  {  typeof(ChannelBase) });
                    _instance = constructor.Invoke(new [] { client.Channel }) as T;
                }
                return _instance;
            }
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Lifecycle;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents an implementation of <see cref="IClientFor{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="ClientBase"/>.</typeparam>
    [Singleton]
    public class ClientFor<T> : IClientFor<T>
        where T : ClientBase
    {
        readonly IConnectedHeads _connectedHeads;
        T _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientFor{T}"/> class.
        /// </summary>
        /// <param name="connectedHeads"><see cref="IConnectedHeads"/> to use for maintaining connection state.</param>
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
                    client.Disconnected += _ => _instance = null;

                    var constructor = type.GetConstructor(new Type[] { typeof(ChannelBase) });
                    _instance = constructor.Invoke(new[] { client.Channel }) as T;
                }

                return _instance;
            }
        }
    }
}
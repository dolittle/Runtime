// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Runtime.Reflection;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents an implementation of <see cref="IClientManager"/>.
    /// </summary>
    public class ClientManager : IClientManager
    {
        readonly ICallInvokerManager _callInvokerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientManager"/> class.
        /// </summary>
        /// <param name="callInvokerManager"><see cref="ICallInvokerManager"/> to get <see cref="CallInvoker"/> from.</param>
        public ClientManager(ICallInvokerManager callInvokerManager)
        {
            _callInvokerManager = callInvokerManager;
        }

        /// <inheritdoc/>
        public ClientBase Get(Type type, string host = default, int port = default)
        {
            ThrowIfTypeDoesNotImplementClientBase(type);
            var constructor = type.GetConstructor(new[] { typeof(CallInvoker) });
            ThrowIfMissingExpectedConstructorClientType(type, constructor);

            return constructor.Invoke(new[] { _callInvokerManager.GetFor(type, host, port) }) as ClientBase;
        }

        /// <inheritdoc/>
        public TClient Get<TClient>(string host = default, int port = default)
            where TClient : ClientBase => Get(typeof(TClient), host, port) as TClient;

        void ThrowIfTypeDoesNotImplementClientBase(Type type)
        {
            if (!type.Implements(typeof(ClientBase)))
            {
                throw new TypeDoesNotImplementClientBase(type);
            }
        }

        void ThrowIfMissingExpectedConstructorClientType(Type type, ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new MissingExpectedConstructorForClientType(type);
            }
        }
    }
}
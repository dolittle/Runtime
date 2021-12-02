// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Server.Handshake
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly HandshakeService _handshakeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="handshakeService">The <see cref="HandshakeService"/>.</param>
        public RuntimeServices(HandshakeService handshakeService)
        {
            _handshakeService = handshakeService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Server";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new Service[]
            {
                new(_handshakeService, Dolittle.Runtime.Handshake.Contracts.Handshake.BindService(_handshakeService), Dolittle.Runtime.Handshake.Contracts.Handshake.Descriptor)
            };
    }
}

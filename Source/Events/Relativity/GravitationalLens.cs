/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Server;
using Dolittle.Serialization.Protobuf;
using Dolittle.Runtime.Events.Relativity.Grpc;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the binder that gives us the <see cref="QuantumTunnelService"/>
    /// </summary>
    /// <remarks>
    /// In order to observe black holes and its event horizons, one can do so through observing the gravitational lens.
    /// The service exposed enables the server to see these
    /// </remarks>
    public class GravitationalLens : ICanBindInteractionServices
    {
        readonly IEventHorizon _eventHorizon;
        readonly ISerializer _serializer;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="GravitationalLens"/>
        /// </summary>
        /// <param name="eventHorizon"></param>
        /// <param name="serializer"><see cref="ISerializer"/> for serializing payloads</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for dealing with <see cref="IExecutionContext"/></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public GravitationalLens(
            IEventHorizon eventHorizon,
            ISerializer serializer,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _serializer = serializer;
            _executionContextManager = executionContextManager;
            _logger = logger;
            _eventHorizon = eventHorizon;
        }

        /// <inheritdoc/>
        public IEnumerable<ServerServiceDefinition> BindServices()
        {
            var service = new QuantumTunnelServiceImplementation(_eventHorizon, _serializer, _executionContextManager, _logger);
            return new ServerServiceDefinition[] {
                QuantumTunnelService.BindService(service)
            };
        }
    }
}
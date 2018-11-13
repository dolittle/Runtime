/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Relativity.Protobuf;
using Dolittle.Runtime.Events.Relativity.Protobuf.Conversion;
using Dolittle.Serialization.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity.Grpc
{
    /// <summary>
    /// Represents an implementation of the <see cref="QuantumTunnelService.QuantumTunnelServiceBase"/>
    /// </summary>
    public class QuantumTunnelServiceImplementation : QuantumTunnelService.QuantumTunnelServiceBase
    {
        readonly IEventHorizon _eventHorizon;
        readonly ISerializer _serializer;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="QuantumTunnelServiceImplementation"/>
        /// </summary>
        /// <param name="eventHorizon"><see cref="IEventHorizon"/> to work with</param>
        /// <param name="serializer"><see cref="ISerializer"/> to be used for serialization</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for dealing with <see cref="ExecutionContext"/></param>
        /// <param name="fetchUnprocessedCommits"><see cref="IFetchUnprocessedCommits"/> for fetching unprocessed commits</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public QuantumTunnelServiceImplementation(
            IEventHorizon eventHorizon,
            ISerializer serializer,
            IExecutionContextManager executionContextManager,
            IFetchUnprocessedCommits fetchUnprocessedCommits,
            ILogger logger)
        {
            _eventHorizon = eventHorizon;
            _serializer = serializer;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Open(OpenTunnel request, IServerStreamWriter<Dolittle.Runtime.Events.Relativity.Protobuf.CommittedEventStreamWithContext> responseStream, ServerCallContext context)
        {
            try
            {
                var tunnel = new QuantumTunnel(_serializer, responseStream, context.CancellationToken, _logger);
                var application = request.Application.ToConcept<Application>();
                var boundedContext = request.BoundedContext.ToConcept<BoundedContext>();
                var events = request
                    .Events
                    .Select(@event => @event.ToArtifact())
                    .ToArray();
                var tenantOffsets = request.Offsets.ToTenantOffsets();

                var subscription = new EventParticleSubscription(events);

                _logger.Information($"Opening up a quantum tunnel for bounded context '{boundedContext}' in application '{application}'");

                var singularity = new Singularity(application, boundedContext, tunnel, subscription);
                _eventHorizon.GravitateTowards(singularity, tenantOffsets);
                tunnel.Collapsed += qt => _eventHorizon.Collapse(singularity);

                await tunnel.Open(request.Offsets.ToTenantOffsets());

                _logger.Information($"Quantum tunnel collapsed for bounded context '{boundedContext}' in application '{application}'");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Problems opening tunnel");
            }

            await Task.CompletedTask;
        }

    }
}
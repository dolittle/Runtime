// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Events.Relativity.Microservice;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Relativity.Protobuf.Conversion;
using Dolittle.Runtime.Protobuf;
using Dolittle.Serialization.Protobuf;
using Grpc.Core;
using static Dolittle.Events.Relativity.Microservice.QuantumTunnelService;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of the <see cref="QuantumTunnelServiceBase"/>.
    /// </summary>
    public class QuantumTunnelServiceImplementation : QuantumTunnelServiceBase
    {
        readonly IEventHorizon _eventHorizon;
        readonly ISerializer _serializer;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuantumTunnelServiceImplementation"/> class.
        /// </summary>
        /// <param name="eventHorizon"><see cref="IEventHorizon"/> to work with.</param>
        /// <param name="serializer"><see cref="ISerializer"/> to be used for serialization.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public QuantumTunnelServiceImplementation(
            IEventHorizon eventHorizon,
            ISerializer serializer,
            ILogger logger)
        {
            _eventHorizon = eventHorizon;
            _serializer = serializer;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Open(OpenTunnel request, IServerStreamWriter<CommittedEventStreamWithContext> responseStream, ServerCallContext context)
        {
            try
            {
                using (var tunnel = new QuantumTunnel(_serializer, responseStream, _logger, context.CancellationToken))
                {
                    var application = request.Application.To<Dolittle.Applications.Application>();
                    var boundedContext = request.BoundedContext.To<BoundedContext>();
                    var events = request
                        .Events
                        .Select(@event => @event.ToArtifact())
                        .ToArray();
                    var tenantOffsets = request.Offsets.ToTenantOffsets();

                    var subscription = new EventParticleSubscription(events);

                    _logger.Information($"Opening up a quantum tunnel for bounded context '{boundedContext}' in application '{application}'");

                    var singularity = new Singularity(application, boundedContext, tunnel, subscription);
                    _eventHorizon.GravitateTowards(singularity, tenantOffsets);
                    tunnel.Collapsed += _ => _eventHorizon.Collapse(singularity);

                    await tunnel.Open(request.Offsets.ToTenantOffsets()).ConfigureAwait(false);

                    _logger.Information($"Quantum tunnel collapsed for bounded context '{boundedContext}' in application '{application}'");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Problems opening tunnel");
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
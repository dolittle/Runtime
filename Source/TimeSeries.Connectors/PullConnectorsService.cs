// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Scheduling;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static Dolittle.TimeSeries.Connectors.Runtime.PullConnectors;
using grpc = Dolittle.TimeSeries.Connectors.Runtime;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Represents an implementation of <see cref="PullConnectorsBase"/>.
    /// </summary>
    public class PullConnectorsService : PullConnectorsBase
    {
        readonly IPullConnectors _pullConnectors;
        readonly ITimers _timers;
        readonly ITagDataPointCoordinator _tagDataPointCoordinator;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PullConnectorsService"/> class.
        /// </summary>
        /// <param name="pullConnectors">Actual <see cref="IPullConnectors"/>.</param>
        /// <param name="timers"><see cref="ITimers"/> for scheduling work.</param>
        /// <param name="tagDataPointCoordinator"><see cref="ITagDataPointCoordinator"/> for coordinator datapoints.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PullConnectorsService(
            IPullConnectors pullConnectors,
            ITimers timers,
            ITagDataPointCoordinator tagDataPointCoordinator,
            ILogger logger)
        {
            _pullConnectors = pullConnectors;
            _timers = timers;
            _tagDataPointCoordinator = tagDataPointCoordinator;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task Connect(grpc.PullConnector request, IServerStreamWriter<grpc.PullRequest> responseStream, ServerCallContext context)
        {
            var id = request.Id.ToGuid();
            var pullConnector = new PullConnector(id, request.Name, request.Interval);

            ITimer timer = null;

            try
            {
                _pullConnectors.Register(pullConnector);

                timer = _timers.Every(pullConnector.Interval, () =>
                {
                    var pullRequest = new grpc.PullRequest();
                    responseStream.WriteAsync(pullRequest);
                });

                context.CancellationToken.ThrowIfCancellationRequested();
                context.CancellationToken.WaitHandle.WaitOne();
            }
            finally
            {
                timer?.Stop();
                timer?.Dispose();

                _pullConnectors.Unregister(pullConnector);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task<Empty> Write(grpc.WriteMessage request, ServerCallContext context)
        {
            var connectorId = request.ConnectorId.To<ConnectorId>();
            if (!_pullConnectors.Has(connectorId)) return Task.FromResult(new Empty());
            var connector = _pullConnectors.GetById(connectorId);
            _tagDataPointCoordinator.Handle(connector.Name, request.Data);

            return Task.FromResult(new Empty());
        }
    }
}
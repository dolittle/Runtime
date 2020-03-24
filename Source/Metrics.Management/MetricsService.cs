// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Metrics.Management;
using Grpc.Core;
using Prometheus;
using static contracts::Dolittle.Runtime.Metrics.Management.Metrics;

namespace Dolittle.Runtime.Metrics.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="MetricsBase"/>.
    /// </summary>
    public class MetricsService : MetricsBase
    {
        readonly CollectorRegistry _collectorRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsService"/> class.
        /// </summary>
        /// <param name="collectorRegistry">The <see cref="CollectorRegistry"/>.</param>
        public MetricsService(CollectorRegistry collectorRegistry)
        {
            _collectorRegistry = collectorRegistry;
        }

        /// <inheritdoc/>
        public override async Task GetCollectors(CollectorsRequest request, IServerStreamWriter<CollectorsResponse> responseStream, ServerCallContext context)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            /*
            await _connectedHeads.All.Forward(
                responseStream,
                context,
                _ => _.Heads,
                _ => new ConnectedHead
                {
                    Head = new HeadInfo
                    {
                        HeadId = _.HeadId.ToProtobuf(),
                        Host = _.Host,
                        Runtime = _.Runtime,
                        Version = _.Version,
                        ConnectionTime = Timestamp.FromDateTimeOffset(_.ConnectionTime)
                    },
                    ConnectionTime = _.ConnectionTime.ToUnixTimeMilliseconds()
                }).ConfigureAwait(false);*/
        }
    }
}
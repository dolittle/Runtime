// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Logging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static contracts::Dolittle.Runtime.TimeSeries.Connectors.PushConnectors;
using grpc = contracts::Dolittle.Runtime.TimeSeries.Connectors;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Represents an implementation of <see cref="PushConnectorsBase"/>.
    /// </summary>
    public class PushConnectorsService : PushConnectorsBase
    {
        readonly IPushConnectors _pushConnectors;
        readonly ILogger _logger;
        readonly ITagDataPointCoordinator _tagDataPointCoordinator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushConnectorsService"/> class.
        /// </summary>
        /// <param name="pushConnectors">Actual <see cref="IPushConnectors"/>.</param>
        /// <param name="tagDataPointCoordinator"><see cref="ITagDataPointCoordinator"/> for coordinator datapoints.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PushConnectorsService(
            IPushConnectors pushConnectors,
            ITagDataPointCoordinator tagDataPointCoordinator,
            ILogger logger)
        {
            _pushConnectors = pushConnectors;
            _tagDataPointCoordinator = tagDataPointCoordinator;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<Empty> Open(IAsyncStreamReader<grpc.PushTagDataPoints> requestStream, ServerCallContext context)
        {
            var pushConnectorIdAsString = context.RequestHeaders.SingleOrDefault(_ => string.Equals(_.Key, "pushconnectorid", StringComparison.InvariantCultureIgnoreCase))?.Value;
            if (string.IsNullOrEmpty(pushConnectorIdAsString)) throw new MissingConnectorIdentifierOnRequestHeader();
            var pushConnectorName = context.RequestHeaders.SingleOrDefault(_ => string.Equals(_.Key, "pushconnectorname", StringComparison.InvariantCultureIgnoreCase))?.Value;
            if (string.IsNullOrEmpty(pushConnectorName)) throw new MissingConnectorNameOnRequestHeader();
            var id = (ConnectorId)Guid.Parse(pushConnectorIdAsString);

            PushConnector pushConnector = null;
            try
            {
                _logger.Debug($"Register connector : '{pushConnectorName}' with Id: '{id}'");
                pushConnector = new PushConnector(id, pushConnectorName);
                _pushConnectors.Register(pushConnector);

                while (await requestStream.MoveNext().ConfigureAwait(false))
                {
                    _tagDataPointCoordinator.Handle(pushConnector.Name, requestStream.Current.DataPoints);
                }
            }
            finally
            {
                if (pushConnector != null) _pushConnectors.Unregister(pushConnector);
            }

            return new Empty();
        }
    }
}
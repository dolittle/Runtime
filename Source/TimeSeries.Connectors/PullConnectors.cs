// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using Dolittle.Lifecycle;
using Dolittle.Logging;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Represent an implementation of <see cref="IPullConnectors"/>.
    /// </summary>
    [Singleton]
    public class PullConnectors : IPullConnectors
    {
        readonly ConcurrentDictionary<ConnectorId, PullConnector> _connectors = new ConcurrentDictionary<ConnectorId, PullConnector>();
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PullConnectors"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PullConnectors(
            ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Register(PullConnector connector)
        {
            _logger.Information($"Register '{connector.Id}'");
            _connectors[connector.Id] = connector;
        }

        /// <inheritdoc/>
        public bool Has(ConnectorId connectorId)
        {
            return _connectors.ContainsKey(connectorId);
        }

        /// <inheritdoc/>
        public PullConnector GetById(ConnectorId connectorId)
        {
            return _connectors[connectorId];
        }

        /// <inheritdoc/>
        public void Unregister(PullConnector connector)
        {
            if (_connectors.ContainsKey(connector.Id))
            {
                _logger.Information($"Unregister '{connector.Id}'");
                _connectors.TryRemove(connector.Id, out PullConnector _);
            }
            else
            {
                _logger.Warning($"Connector with id '{connector.Id}' is not registered");
            }
        }
    }
}
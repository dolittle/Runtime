// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using Dolittle.Lifecycle;
using Dolittle.Logging;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Represent an implementation of <see cref="IPushConnectors"/>.
    /// </summary>
    [Singleton]
    public class PushConnectors : IPushConnectors
    {
        readonly ConcurrentDictionary<ConnectorId, PushConnector> _connectors = new ConcurrentDictionary<ConnectorId, PushConnector>();
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushConnectors"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PushConnectors(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Register(PushConnector connector)
        {
            _logger.Debug($"Register '{connector.Id}'");
            _connectors[connector.Id] = connector;
        }

        /// <inheritdoc/>
        public bool Has(ConnectorId connectorId)
        {
            return _connectors.ContainsKey(connectorId);
        }

        /// <inheritdoc/>
        public PushConnector GetById(ConnectorId connectorId)
        {
            return _connectors[connectorId];
        }

        /// <inheritdoc/>
        public void Unregister(PushConnector connector)
        {
            if (_connectors.ContainsKey(connector.Id))
            {
                _logger.Debug($"Unregister '{connector.Id}'");
                _connectors.TryRemove(connector.Id, out PushConnector _);
            }
            else
            {
                _logger.Warning($"Connector with id '{connector.Id}' is not registered");
            }
        }
    }
}
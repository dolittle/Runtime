// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Defines a system for working with all available stream connectors.
    /// </summary>
    public interface IPushConnectors
    {
        /// <summary>
        /// Register a stream connector.
        /// </summary>
        /// <param name="pushConnector"><see cref="PushConnector"/> to register.</param>
        void Register(PushConnector pushConnector);

        /// <summary>
        /// Check if is registered <see cref="PushConnector"/> by its identifier.
        /// </summary>
        /// <param name="connectorId"><see cref="ConnectorId"/> to check.</param>
        /// <returns>true if it is registered, false if not.</returns>
        bool Has(ConnectorId connectorId);

        /// <summary>
        /// Get a <see cref="PushConnector"/> by its identifier.
        /// </summary>
        /// <param name="connectorId"><see cref="ConnectorId"/> to get.</param>
        /// <returns><see cref="PushConnector"/>.</returns>
        PushConnector GetById(ConnectorId connectorId);

        /// <summary>
        /// Unregister a stream connector.
        /// </summary>
        /// <param name="pushConnector"><see cref="PushConnector"/> to unregister.</param>
        void Unregister(PushConnector pushConnector);
    }
}
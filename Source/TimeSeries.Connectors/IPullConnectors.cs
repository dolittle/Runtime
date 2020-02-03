// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Defines a system for working with all available pull connectors.
    /// </summary>
    public interface IPullConnectors
    {
        /// <summary>
        /// Register a <see cref="PullConnector"/>.
        /// </summary>
        /// <param name="pullConnector"><see cref="PullConnector"/> to register.</param>
        void Register(PullConnector pullConnector);

        /// <summary>
        /// Check if is registered <see cref="PullConnector"/> by its identifier.
        /// </summary>
        /// <param name="connectorId"><see cref="ConnectorId"/> to check.</param>
        /// <returns>True if it is registered, false if not.</returns>
        bool Has(ConnectorId connectorId);

        /// <summary>
        /// Get a <see cref="PullConnector"/> by its identifier.
        /// </summary>
        /// <param name="connectorId"><see cref="ConnectorId"/> to get.</param>
        /// <returns><see cref="PullConnector"/>.</returns>
        PullConnector GetById(ConnectorId connectorId);

        /// <summary>
        /// Unregister a <see cref="PullConnector"/>.
        /// </summary>
        /// <param name="pullConnector"><see cref="PullConnector"/> to unregister.</param>
        void Unregister(PullConnector pullConnector);
    }
}
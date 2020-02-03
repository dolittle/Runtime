// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Defines a push connector.
    /// </summary>
    public class PushConnector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PushConnector"/> class.
        /// </summary>
        /// <param name="id">Unique <see cref="ConnectorId"/>.</param>
        /// <param name="name">Name of the connector.</param>
        public PushConnector(
            ConnectorId id,
            string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets the <see cref="ConnectorId"/> for the <see cref="PullConnector"/>.
        /// </summary>
        public ConnectorId Id { get; }

        /// <summary>
        /// Gets the name of the <see cref="PullConnector"/>.
        /// </summary>
        public string Name { get; }
   }
}
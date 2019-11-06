/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Configuration;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents the <see cref="IConfigurationObject"/> for Metrics
    /// </summary>
    public class MetricsConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MetricsConfiguration"/>
        /// </summary>
        /// <param name="port">Port to expose the metrics server on</param>
        public MetricsConfiguration(int port)
        {
            Port = port;
        }

        /// <summary>
        /// Gets the port to expose the metrics server on
        /// </summary>
        public int Port { get; }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Configuration;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents the <see cref="IConfigurationObject"/> for Metrics.
    /// </summary>
    public class MetricsConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsConfiguration"/> class.
        /// </summary>
        /// <param name="port">Port to expose the metrics server on.</param>
        public MetricsConfiguration(int port)
        {
            Port = port;
        }

        /// <summary>
        /// Gets the port to expose the metrics server on.
        /// </summary>
        public int Port { get; }
    }
}
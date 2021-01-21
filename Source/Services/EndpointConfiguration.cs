// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the configuration typically used by a <see cref="IEndpoint"/>.
    /// </summary>
    public class EndpointConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointConfiguration"/> class.
        /// </summary>
        public EndpointConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointConfiguration"/> class.
        /// </summary>
        /// <param name="port">Port to run the host on.</param>
        public EndpointConfiguration(int port) => Port = port;

        /// <summary>
        /// Gets or sets a value indicating whether or not the interaction server is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the port to use for exposing the <see cref="IEndpoint"/> on.
        /// </summary>
        public int Port { get; set; } = 50051;
    }
}
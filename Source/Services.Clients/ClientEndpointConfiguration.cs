// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents the configuration typically used to connect clients to a host.
    /// </summary>
    public class ClientEndpointConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEndpointConfiguration"/> class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">Port to run the host on.</param>
        public ClientEndpointConfiguration(string host, int port)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Gets the host to connect to.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the port to connect to.
        /// </summary>
        public int Port { get; }
    }
}
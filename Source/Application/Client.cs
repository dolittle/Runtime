/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents a connected client
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Client"/>
        /// </summary>
        /// <param name="clientId"><see cref="ClientId"/> of the client</param>
        /// <param name="host">Hostname of the client</param>
        /// <param name="port">TCP port to connect back to the client</param>
        /// <param name="runtime">Runtime information from the client</param>
        /// <param name="connectionTime">Time of when client was connected</param>
        public Client(
            ClientId clientId,
            string host,
            uint port,
            string runtime,
            DateTimeOffset connectionTime)
        {
            ClientId = clientId;
            Host = host;
            Port = port;
            Runtime = runtime;
            ConnectionTime = connectionTime;
        }

        /// <summary>
        /// Gets the <see cref="ClientId"/> for the client
        /// </summary>
        public ClientId ClientId { get; }

        /// <summary>
        /// Gets the hostname for the client for connecting to it
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the TCP port for the client for connecting to it
        /// </summary>
        public uint Port { get; }

        /// <summary>
        /// Gets a string with runtime information from the client
        /// </summary>
        public string Runtime { get; }

        /// <summary>
        /// Gets the time when client was connected
        /// </summary>
        public DateTimeOffset ConnectionTime { get; }
    }
}
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents a connected head
    /// </summary>
    public class Head
    {
        /// <summary>
        /// Event that occurs when the client is disconnected
        /// </summary>
        public event HeadDisconnected Disconnected = (c) => {};

        /// <summary>
        /// Initializes a new instance of <see cref="Head"/>
        /// </summary>
        /// <param name="headId"><see cref="HeadId"/> of the client</param>
        /// <param name="host">Hostname of the client</param>
        /// <param name="port">TCP port to connect back to the client</param>
        /// <param name="runtime">Runtime information from the client</param>
        /// <param name="servicesByName">Names of services exposed</param>
        /// <param name="connectionTime">Time of when client was connected</param>
        public Head(
            HeadId headId,
            string host,
            int port,
            string runtime,
            IEnumerable<string> servicesByName,
            DateTimeOffset connectionTime)
        {
            HeadId = headId;
            Host = host;
            Port = port;
            Runtime = runtime;
            ServicesByName = servicesByName;
            ConnectionTime = connectionTime;

            var keepAliveTime = new ChannelOption("grpc.keepalive_time", 1000);
            var keepAliveTimeout = new ChannelOption("grpc.keepalive_timeout_ms", 500);
            var keepAliveWithoutCalls = new ChannelOption("grpc.keepalive_permit_without_calls", 1);

            Channel = new Channel(host, (int) port, ChannelCredentials.Insecure, new []
            {
                keepAliveTime,
                keepAliveTimeout,
                keepAliveWithoutCalls
            });
        }

        /// <summary>
        /// Gets the <see cref="HeadId"/> for the client
        /// </summary>
        public HeadId HeadId {  get; }

        /// <summary>
        /// Gets the hostname for the client for connecting to it
        /// </summary>
        public string Host {  get; }

        /// <summary>
        /// Gets the TCP port for the client for connecting to it
        /// </summary>
        public int Port {  get; }

        /// <summary>
        /// Gets a string with runtime information from the client
        /// </summary>
        public string Runtime {  get; }

        /// <summary>
        /// Gets the services represented by the client by their name
        /// </summary>
        public IEnumerable<string> ServicesByName { get; }

        /// <summary>
        /// Gets the channel for the client
        /// </summary>
        public ChannelBase Channel {  get; }

        /// <summary>
        /// Gets the time when client was connected
        /// </summary>
        public DateTimeOffset ConnectionTime { get; }

        internal void OnDisconnected()
        {
            Disconnected(this);
        }
    }
}
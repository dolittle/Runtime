// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Services;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents a connected head.
    /// </summary>
    public class Head
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Head"/> class.
        /// </summary>
        /// <param name="headId"><see cref="HeadId"/> of the client.</param>
        /// <param name="host">The hostname of the <see cref="Head"/>.</param>
        /// <param name="runtime">Runtime information from the client.</param>
        /// <param name="version">Version of the <see cref="Head"/>.</param>
        /// <param name="connectionTime">Time of when client was connected.</param>
        public Head(
            HeadId headId,
            string host,
            string runtime,
            string version,
            DateTimeOffset connectionTime)
        {
            HeadId = headId;
            Host = host;
            Runtime = runtime;
            Version = version;
            ConnectionTime = connectionTime;
        }

        /// <summary>
        /// Event that occurs when the client is disconnected
        /// </summary>
        public event HeadDisconnected Disconnected = _ => { };

        /// <summary>
        /// Gets the <see cref="HeadId"/> for the client.
        /// </summary>
        public HeadId HeadId { get; }

        /// <summary>
        /// Gets the hostname for the <see cref="Head"/>.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets a string with runtime information from the client.
        /// </summary>
        public string Runtime { get; }

        /// <summary>
        /// Gets the version of the <see cref="Head"/>.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the time when client was connected.
        /// </summary>
        public DateTimeOffset ConnectionTime { get; }

        /// <summary>
        /// Called when a head gets disconnected.
        /// </summary>
        internal void OnDisconnected()
        {
            Disconnected(this);
        }
    }
}
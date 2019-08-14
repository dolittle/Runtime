/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Grpc.Core;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Defines a host for hosting services for a specific purpose
    /// </summary>
    public interface IHost : IDisposable
    {
        /// <summary>
        /// Start the host with the configuration for it
        /// </summary>
        /// <param name="identifier">Identifier of the type of services host is providing</param>
        /// <param name="configuration"><see cref="HostConfiguration"/> for the host</param>
        /// <param name="services">Collection of <see cref="ServerServiceDefinition"/> to host</param>
        void Start(string identifier, HostConfiguration configuration, IEnumerable<ServerServiceDefinition> services);
    }
}
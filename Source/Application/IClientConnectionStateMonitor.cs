/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Defines a system that is capable of tracking status for a given <see cref="ChannelBase"/>
    /// </summary>
    public interface IClientConnectionStateMonitor
    {
        /// <summary>
        /// Monitor a specific <see cref="Client"/> for state changes
        /// </summary>
        /// <param name="client"><see cref="Client"/> to track</param>
        void Monitor(Client client);
    }
}
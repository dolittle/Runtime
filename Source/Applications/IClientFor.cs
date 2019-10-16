/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Grpc.Core;

namespace Dolittle.Runtime.Applications
{
    /// <summary>
    /// Defines an interface for getting a gRPC client proxy for a service
    /// </summary>
    /// <typeparam name="T">Type of client - based on <see cref="ClientBase"/></typeparam>
    public interface IClientFor<T> where T:ClientBase
    {
        /// <summary>
        /// Gets the actual instance of the client
        /// </summary>
        T   Instance { get; }
    }
}
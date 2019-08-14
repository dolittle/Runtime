/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Defines a system that manages all the <see cref="IHost">hosts</see>
    /// </summary>
    public interface IHosts : IDisposable
    {
        /// <summary>
        /// Start all the hosts
        /// </summary>
        void Start();
    }
}
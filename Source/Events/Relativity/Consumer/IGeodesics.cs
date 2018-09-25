/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Applications;
using Dolittle.Concepts;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines a system that maintains the offset in the path from an <see cref="IEventHorizon"/>
    /// </summary>
    /// <remarks>
    /// An observer travelling along a geodesic path may remain in motion forever, or the path 
    /// may terminate after a finite amount of time. Paths that carry on indefinitely are called 
    /// complete geodesics, and those that stop abruptly, incomplete geodesics.
    /// </remarks>
    public interface IGeodesics : IDisposable
    {
        /// <summary>
        /// Get path offset for a specific <see cref="Application"/> and <see cref="BoundedContext"/> for a <see cref="TenantId"/>
        /// </summary>
        /// <param name="key"><see cref="EventHorizonKey"/> to get for</param>
        /// <returns>The offset</returns>
        ulong GetOffset(EventHorizonKey key);

        /// <summary>
        /// Set path offset for a specific <see cref="Application"/> and <see cref="BoundedContext"/> for a <see cref="TenantId"/>
        /// </summary>
        /// <param name="key"><see cref="EventHorizonKey"/> to set for</param>
        /// <param name="offset">Offset to set</param>
        void SetOffset(EventHorizonKey key, ulong offset);
    }
}
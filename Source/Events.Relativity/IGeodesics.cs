/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Applications;
using Dolittle.Runtime.Tenancy;

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
    public interface IGeodesics
    {
        /// <summary>
        /// Get path offset for a specific <see cref="Application"/> and <see cref="BoundedContext"/> for a <see cref="TenantId"/>
        /// </summary>
        /// <param name="application"><see cref="Application"/> to get for</param>
        /// <param name="BoundedContext"><see cref="BoundedContext"/> to get for</param>
        /// <param name="tenant"><see cref="TenantId"/> to get for</param>
        /// <returns>Offset into the path</returns>
        ulong GetPathOffsetFor(Application application, BoundedContext BoundedContext, TenantId tenant);

        /// <summary>
        /// Set path offset for a specific <see cref="Application"/> and <see cref="BoundedContext"/> for a <see cref="TenantId"/>
        /// </summary>
        /// <param name="application"><see cref="Application"/> to set for</param>
        /// <param name="BoundedContext"><see cref="BoundedContext"/> to get for</param>
        /// <param name="tenant"><see cref="TenantId"/> to set for</param>
        /// <param name="offset">Offset into the path to set</param>
        void StorePathOffsetFor(Application application, BoundedContext BoundedContext, TenantId tenant, ulong offset);
    }

    /// <summary>
    /// 
    /// </summary>
    public class Geodesics : IGeodesics
    {
        /// <inheritdoc/>
        public ulong GetPathOffsetFor(Application application, BoundedContext BoundedContext, TenantId tenant)
        {
            return 0;
        }

        /// <inheritdoc/>
        public void StorePathOffsetFor(Application application, BoundedContext BoundedContext, TenantId tenant, ulong offset)
        {
            
        }
    }
}
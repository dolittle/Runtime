// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a system representing all bound <see cref="Service">services</see> in the system.
    /// </summary>
    public interface IBoundServices
    {
        /// <summary>
        /// Register all services for a specific type - this overwrites any that are already set.
        /// </summary>
        /// <param name="type"><see cref="ServiceType"/> to register for.</param>
        /// <param name="services">Collection of <see cref="Service"/>.</param>
        void Register(ServiceType type, IEnumerable<Service> services);

        /// <summary>
        /// Check if there are bound services for a specific <see cref="ServiceType"/>.
        /// </summary>
        /// <param name="type"><see cref="ServiceType"/> to check if has services.</param>
        /// <returns>True if there are services, false if not.</returns>
        bool HasFor(ServiceType type);

        /// <summary>
        /// Get all <see cref="Service"/> for a specific <see cref="ServiceType"/>.
        /// </summary>
        /// <param name="type"><see cref="ServiceType"/> to get for.</param>
        /// <returns>Collection of <see cref="Service"/>.</returns>
        IEnumerable<Service> GetFor(ServiceType type);
    }
}
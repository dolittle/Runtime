// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines the generic interface for binding gRPC services.
    /// </summary>
    public interface ICanBindServices
    {
        /// <summary>
        /// Gets the identifier of the <see cref="ServiceAspect"/> the <see cref="Service">services</see>
        /// represent.
        /// </summary>
        ServiceAspect Aspect { get; }

        /// <summary>
        /// Binds the services and returns the <see cref="Service"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{Service}">Collection of </see>.</returns>
        IEnumerable<Service> BindServices();
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a system that can provide information about a service type.
    /// </summary>
    public interface IRepresentServiceType
    {
        /// <summary>
        /// Gets the identifier of the <see cref="ServiceType"/>.
        /// </summary>
        ServiceType Identifier { get; }

        /// <summary>
        /// Gets the binding interface, must implement <see cref="ICanBindServices"/>.
        /// </summary>
        Type BindingInterface { get; }

        /// <summary>
        /// Gets the <see cref="EndpointVisibility">type of endpoint</see> this is.
        /// </summary>
        EndpointVisibility Visibility { get; }
    }
}
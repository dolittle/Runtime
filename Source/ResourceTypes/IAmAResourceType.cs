// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.ResourceTypes
{
    /// <summary>
    /// Defines a resource type and its services.
    /// </summary>
    public interface IAmAResourceType
    {
        /// <summary>
        /// Gets the name of the resource type.
        /// </summary>
        ResourceType Name { get; }

        /// <summary>
        /// Gets the services related to the <see cref="ResourceType"/>.
        /// </summary>
        IEnumerable<Type> Services { get; }
    }
}
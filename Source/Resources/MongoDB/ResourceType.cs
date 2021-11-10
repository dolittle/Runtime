// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Represents a <see cref="IAmAResourceType">resource type</see> for the Mongo Db resource.
    /// </summary>
    /// <remarks>The name is currently readModels to support for legacy cases.</remarks>
    public class ResourceType : IAmAResourceType
    {
        /// <summary>
        /// The <see cref="ResourceTypes.ResourceType"/> name.
        /// </summary>
        public static ResourceTypes.ResourceType ResourceTypeName => "readModels";
        
        /// <inheritdoc/>
        public ResourceTypes.ResourceType Name => ResourceTypeName;

        /// <inheritdoc/>
        public IEnumerable<Type> Services { get; } = new[] { typeof(IResource) };
    }
}

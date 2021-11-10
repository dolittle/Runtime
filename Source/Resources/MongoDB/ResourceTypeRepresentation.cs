// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <inheritdoc/>
    public class ResourceTypeRepresentation : IRepresentAResourceType
    {
        static readonly IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
        {
            { typeof(IResource), typeof(Resource) }
        };

        /// <inheritdoc/>
        public ResourceTypes.ResourceType Type => ResourceType.ResourceTypeName;

        /// <inheritdoc/>
        public ResourceTypeImplementation ImplementationName => "n/a";

        /// <inheritdoc/>
        public Type ConfigurationObjectType => typeof(Configuration);

        /// <inheritdoc/>
        public IDictionary<Type, Type> Bindings => _bindings;
    }
}

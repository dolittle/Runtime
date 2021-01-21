// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.IO.Tenants
{
    /// <summary>
    /// Represents a <see cref="IRepresentAResourceType">resource type representation</see> for <see cref="Files"/>.
    /// </summary>
    public class FilesResourceTypeRepresentation : IRepresentAResourceType
    {
        static readonly IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
        {
            { typeof(IFiles), typeof(Files) },
        };

        /// <inheritdoc/>
        public ResourceType Type => "files";

        /// <inheritdoc/>
        public ResourceTypeImplementation ImplementationName => "Files";

        /// <inheritdoc/>
        public Type ConfigurationObjectType => typeof(FilesConfiguration);

        /// <inheritdoc/>
        public IDictionary<Type, Type> Bindings => _bindings;
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.MongoDB.State;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Projections.Store.MongoDB
{
    /// <inheritdoc/>
    public class ProjectionResourceTypeRepresentation : IRepresentAResourceType
    {
        static readonly IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
        {
            { typeof(IProjections), typeof(Projections) },
            { typeof(IProjectionDefinitions), typeof(ProjectionDefinitions) },
            { typeof(IProjectionStates), typeof(ProjectionStates) }
        };

        /// <inheritdoc/>
        public ResourceType Type => "projections";

        /// <inheritdoc/>
        public ResourceTypeImplementation ImplementationName => "MongoDB";

        /// <inheritdoc/>
        public Type ConfigurationObjectType => typeof(ProjectionsConfiguration);

        /// <inheritdoc/>
        public IDictionary<Type, Type> Bindings => _bindings;
    }
}

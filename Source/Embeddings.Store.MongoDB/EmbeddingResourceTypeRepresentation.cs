// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.MongoDB.Definition;
using Dolittle.Runtime.Embeddings.Store.MongoDB.State;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB
{
    /// <summary>
    /// Represents an implementation of <see cref="IRepresentAResourceType"/> for the embedding resource type.
    /// </summary>
    public class EmbeddingResourceTypeRepresentation : IRepresentAResourceType
    {
        static readonly IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
        {
            { typeof(IEmbeddings), typeof(Embeddings) },
            { typeof(IEmbeddingDefinitions), typeof(EmbeddingDefinitions) },
            { typeof(IEmbeddingStates), typeof(EmbeddingStates) }
        };

        /// <inheritdoc/>
        public ResourceType Type => "embeddings";

        /// <inheritdoc/>
        public ResourceTypeImplementation ImplementationName => "MongoDB";

        /// <inheritdoc/>
        public Type ConfigurationObjectType => typeof(EmbeddingsConfiguration);

        /// <inheritdoc/>
        public IDictionary<Type, Type> Bindings => _bindings;
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;


namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition;

/// <summary>
/// Represents an implementation of <see cref="IConvertEmbeddingDefinition" />.
/// </summary>
[Singleton]
public class ConvertEmbeddingDefinition : IConvertEmbeddingDefinition
{
    /// <inheritdoc/>
    public Store.Definition.EmbeddingDefinition ToRuntime(EmbeddingDefinition definition)
        => new(
            definition.Embedding,
            definition.Events.Select(_ => new Artifact(_, ArtifactGeneration.First)),
            definition.InitialState);

    /// <inheritdoc/>
    public EmbeddingDefinition ToStored(Store.Definition.EmbeddingDefinition definition)
        => new()
        {
            Embedding = definition.Embedding,
            InitialState = definition.InititalState,
            Events = definition.Events.Select(_ => _.Id.Value).ToArray()
        };
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IEmbeddings" />.
/// </summary>
[Singleton, PerTenant]
public class Embeddings : EmbeddingsConnection, IEmbeddings
{
    const string EmbeddingDefinitionCollectionName = "embedding-definitions";
    readonly IMongoCollection<Definition.EmbeddingDefinition> _embeddingDefinitions;
    static string CollectionNameForEmbedding(EmbeddingId embeddingId) => $"embedding-{embeddingId.Value}";

    /// <summary>
    /// Initializes a new instance of the <see cref="Embeddings"/> class.
    /// </summary>
    /// <param name="connection">The <see cref="DatabaseConnection" />.</param>
    public Embeddings(IDatabaseConnection connection)
        : base(connection)
    {
        _embeddingDefinitions = Database.GetCollection<Definition.EmbeddingDefinition>(EmbeddingDefinitionCollectionName);
    }

    /// <inheritdoc/>
    public Task<IMongoCollection<Definition.EmbeddingDefinition>> GetDefinitions(CancellationToken token)
        => Task.FromResult(_embeddingDefinitions);

    /// <inheritdoc/>
    public Task<IMongoCollection<State.Embedding>> GetStates(EmbeddingId embeddingId, CancellationToken token)
        => Task.FromResult(Database.GetCollection<State.Embedding>(CollectionNameForEmbedding(embeddingId)));
}

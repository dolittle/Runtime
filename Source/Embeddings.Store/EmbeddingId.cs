// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Represents the unique identifier of an embedding.
/// </summary>
/// <param name="Value">The identifier of the embedding as a <see cref="Guid"/>.</param>
public record EmbeddingId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Implicitly convert from a <see cref="Guid"/> to an <see cref="EmbeddingId"/>.
    /// </summary>
    /// <param name="embeddingId">EmbeddingId as a <see cref="Guid"/>.</param>
    public static implicit operator EmbeddingId(Guid embeddingId) => new(embeddingId);

    /// <summary>
    /// Implicitly convert from a <see cref="string"/> to an <see cref="EmbeddingId"/>.
    /// </summary>
    /// <param name="embeddingId">EmbeddingId as a <see cref="string"/>.</param>
    public static implicit operator EmbeddingId(string embeddingId) => new(Guid.Parse(embeddingId));

    /// <summary>
    /// Generates a new <see cref="EmbeddingId"/>.
    /// </summary>
    /// <returns>A randomly generated <see cref="EmbeddingId"/>.</returns>
    public static EmbeddingId New() => Guid.NewGuid();
}
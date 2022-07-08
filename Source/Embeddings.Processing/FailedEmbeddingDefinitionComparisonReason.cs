// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents the reason for why the comparison of a <see cref="EmbeddingDefintion" /> failed.
/// </summary>
public record FailedEmbeddingDefinitionComparisonReason(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts <see cref="string" /> to <see cref="FailedEmbeddingDefinitionComparisonReason" />.
    /// </summary>
    /// <param name="reason">The value.</param>
    public static implicit operator FailedEmbeddingDefinitionComparisonReason(string reason) => new(reason);
}
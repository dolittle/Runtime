// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents the result of the comparison of a <see cref="EmbeddingDefinition" />.
/// </summary>
public class EmbeddingDefinitionComparisonResult
{
    /// <summary>
    /// Creates a successful <see cref="EmbeddingDefinitionComparisonResult" />.
    /// </summary>
    /// <returns>The successful <see cref="EmbeddingDefinitionComparisonResult" />.</returns>
    public static EmbeddingDefinitionComparisonResult Equal => new();

    /// <summary>
    /// Creates a failed <see cref="EmbeddingDefinitionComparisonResult" />.
    /// </summary>
    /// <param name="reason">The reason why they're unequal.</param>
    /// <returns>The failed <see cref="EmbeddingDefinitionComparisonResult" />.</returns>
    public static EmbeddingDefinitionComparisonResult Unequal(FailedEmbeddingDefinitionComparisonReason reason)
        => new(reason);

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingDefinitionComparisonResult"/> class.
    /// </summary>
    EmbeddingDefinitionComparisonResult() => Succeeded = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingDefinitionComparisonResult"/> class.
    /// </summary>
    /// <param name="reason">The <see cref="FailedEmbeddingDefinitionComparisonReason" />.</param>
    EmbeddingDefinitionComparisonResult(FailedEmbeddingDefinitionComparisonReason reason) => FailureReason = reason;

    /// <summary>
    /// Gets a value indicating whether the validation succeeded or not.
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    /// Gets the <see cref="FailedEmbeddingDefinitionComparisonReason" />.
    /// </summary>
    public FailedEmbeddingDefinitionComparisonReason FailureReason { get; }
}
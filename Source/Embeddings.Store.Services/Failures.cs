// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Embeddings.Store.Services;

/// <summary>
/// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Embeddings.
/// </summary>
public static class Failures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'FailedToGetEmbeddingDefinition' failure type.
    /// </summary>
    public static readonly FailureId FailedToGetEmbeddingDefinition = FailureId.Create("c76b3806-8b74-47b0-b169-5e3ff1f5265b");
}
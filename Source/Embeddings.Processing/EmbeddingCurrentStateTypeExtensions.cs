// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Store;
using ContractsProjectionCurrentStateType = Dolittle.Runtime.Projections.Contracts.ProjectionCurrentStateType;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Extensions for <see cref="EmbeddingCurrentStateType"/>.
/// </summary>
public static class EmbeddingCurrentStateTypeExtensions
{
    /// <summary>
    /// Convert to a protobuf representation of <see cref="EmbeddingCurrentStateType"/>.
    /// </summary>
    /// <param name="type"><see cref="EmbeddingCurrentStateType"/> to convert.</param>
    /// <returns>Converted <see cref="ContractsProjectionCurrentStateType"/>.</returns>
    public static ContractsProjectionCurrentStateType ToProtobuf(this EmbeddingCurrentStateType type)
        => type switch
        {
            EmbeddingCurrentStateType.CreatedFromInitialState => ContractsProjectionCurrentStateType.CreatedFromInitialState,
            EmbeddingCurrentStateType.Persisted => ContractsProjectionCurrentStateType.Persisted,
            _ => throw new UnknownEmbeddingCurrentStateType(type),
        };
}
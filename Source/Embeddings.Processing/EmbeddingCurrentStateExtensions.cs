// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Embeddings.Store;
using ContractsProjectionCurrentState = Dolittle.Runtime.Projections.Contracts.ProjectionCurrentState;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Extensions for <see cref="EmbeddingCurrentState"/>.
/// </summary>
public static class EmbeddingCurrentStateExtensions
{
    /// <summary>
    /// Convert to a protobuf representation of <see cref="EmbeddingCurrentState"/>.
    /// </summary>
    /// <param name="state"><see cref="EmbeddingCurrentState"/> to convert.</param>
    /// <returns>Converted <see cref="ContractsProjectionCurrentState"/>.</returns>
    public static ContractsProjectionCurrentState ToProtobuf(this EmbeddingCurrentState state)
        => new()
        {
            Type = state.Type.ToProtobuf(),
            State = state.State,
            Key = state.Key
        };

    /// <summary>
    /// Convert to an <see cref="IEnumerable{T}"/> of protobuf representations of <see cref="EmbeddingCurrentState"/>.
    /// </summary>
    /// <param name="states">The <see cref="IEnumerable{T}"/> of type <see cref="EmbeddingCurrentState"/> to convert.</param>
    /// <returns>Converted <see cref="IEnumerable{T}"/> of type <see cref="ContractsProjectionCurrentState"/>.</returns>
    public static IEnumerable<ContractsProjectionCurrentState> ToProtobuf(this IEnumerable<EmbeddingCurrentState> states)
        => states.Select(_ => _.ToProtobuf());
}
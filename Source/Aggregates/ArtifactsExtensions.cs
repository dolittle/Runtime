// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents conversion extensions for the Aggregate Root artifact types.
    /// </summary>
    public static class ArtifactsExtensions
    {
        /// <summary>
        /// Convert a <see cref="AggregateRootId"/> to <see cref="Artifact"/>.
        /// </summary>
        /// <param name="identifier"><see cref="AggregateRootId"/> to convert from.</param>
        /// <returns>Converted <see cref="Artifact"/>.</returns>
        public static Artifact ToProtobuf(this AggregateRootId identifier) =>
            new() { Id = identifier.Id.Value.ToProtobuf(), Generation = identifier.Generation.Value };

        /// <summary>
        /// Convert a <see cref="Artifact"/> to <see cref="AggregateRootId"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to convert from.</param>
        /// <returns>Converted <see cref="AggregateRootId"/>.</returns>
        public static AggregateRootId ToAggregateRootId(this Artifact artifact) =>
            new(artifact.Id.ToGuid(), artifact.Generation);
    }
}

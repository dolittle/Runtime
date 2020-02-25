// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Extension methods for <see cref="TypePartitionFilterDefinition" />.
    /// </summary>
    public static class TypePartitionFilterDefinitionExtensions
    {
        /// <summary>
        /// Creates a store representation of a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
        /// </summary>
        /// <param name="definition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <returns>The store representation <see cref="TypePartitionFilterDefinition" />.</returns>
        public static TypePartitionFilterDefinition ToStoreRepresentation(this TypeFilterWithEventSourcePartitionDefinition definition) =>
            new TypePartitionFilterDefinition
            {
                TargetStream = definition.TargetStream,
                SourceStream = definition.SourceStream,
                Partitioned = definition.Partitioned,
                Types = definition.Types.Select(_ => _.Value)
            };

        /// <summary>
        /// Creates a store representation of a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
        /// </summary>
        /// <param name="definition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <returns>The store representation <see cref="TypePartitionFilterDefinition" />.</returns>
        public static TypeFilterWithEventSourcePartitionDefinition ToRuntimeRepresentation(this TypePartitionFilterDefinition definition) =>
            new TypeFilterWithEventSourcePartitionDefinition(
                definition.SourceStream,
                definition.TargetStream,
                definition.Types.Cast<ArtifactId>(),
                definition.Partitioned);
    }
}
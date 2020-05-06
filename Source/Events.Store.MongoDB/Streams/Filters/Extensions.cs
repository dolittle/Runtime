// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts the <see cref="IFilterDefinition" /> to <see cref="AbstractFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <returns>Converted <see cref="AbstractFilterDefinition" />.</returns>
        public static AbstractFilterDefinition ToStoreRepresentation(this IFilterDefinition filterDefinition)
        {
            return filterDefinition switch
                {
                    Store.Streams.Filters.TypeFilterWithEventSourcePartitionDefinition definition => definition.ToStoreRepresentation(),
                    Store.Streams.Filters.EventHorizon.PublicFilterDefinition definition => definition.ToStoreRepresentation(),
                    _ => new FilterDefinition(filterDefinition.TargetStream, filterDefinition.SourceStream, filterDefinition.Partitioned),
                };
        }

        /// <summary>
        /// Converts the <see cref="Store.Streams.Filters.EventHorizon.PublicFilterDefinition" /> to <see cref="PublicFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="Store.Streams.Filters.EventHorizon.PublicFilterDefinition" />.</param>
        /// <returns>Converted <see cref="PublicFilterDefinition" />.</returns>
        public static PublicFilterDefinition ToStoreRepresentation(this Store.Streams.Filters.EventHorizon.PublicFilterDefinition filterDefinition) =>
            new PublicFilterDefinition(filterDefinition.TargetStream, filterDefinition.SourceStream);

        /// <summary>
        /// Converts the <see cref="Store.Streams.Filters.TypeFilterWithEventSourcePartitionDefinition" /> to <see cref="TypePartitionFilterDefinition" />.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="Store.Streams.Filters.TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <returns>Converted <see cref="TypePartitionFilterDefinition" />.</returns>
        public static TypePartitionFilterDefinition ToStoreRepresentation(this Store.Streams.Filters.TypeFilterWithEventSourcePartitionDefinition filterDefinition) =>
            new TypePartitionFilterDefinition(filterDefinition.TargetStream, filterDefinition.SourceStream, filterDefinition.Types.Select(_ => _.Value), filterDefinition.Partitioned);
    }
}
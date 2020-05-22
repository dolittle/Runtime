// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;

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
                    TypeFilterWithEventSourcePartitionDefinition definition =>
                        new TypePartitionFilterDefinition(definition.Types.Select(_ => _.Value)),
                    PublicFilterDefinition _ => new RemoteFilterDefinition(),
                    FilterDefinition _ => new RemoteFilterDefinition(),
                    _ => throw new UnsupportedFilterDefinitionType(filterDefinition)
                };
        }
    }
}

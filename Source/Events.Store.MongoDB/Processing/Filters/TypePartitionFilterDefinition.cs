// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    public class TypePartitionFilterDefinition : FilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypePartitionFilterDefinition"/> class.
        /// </summary>
        /// <param name="filterId">The filter id.</param>
        /// <param name="sourceStream">The source stream id.</param>
        /// <param name="types">The event artifact types.</param>
        /// <param name="partitioned">Whether it is partitioned or not.</param>
        public TypePartitionFilterDefinition(Guid filterId, Guid sourceStream, IEnumerable<Guid> types, bool partitioned)
            : base(filterId, sourceStream)
        {
            Types = types;
            Partitioned = partitioned;
        }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactId"> types</see> that this definition filters on.
        /// </summary>
        public IEnumerable<Guid> Types { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this type filter is partitioned.
        /// </summary>
        public bool Partitioned { get; set; }

        /// <inheritdoc/>
        public override IFilterDefinition AsRuntimeRepresentation() =>
            new TypeFilterWithEventSourcePartitionDefinition(
                SourceStream,
                FilterId,
                Types.Cast<ArtifactId>(),
                Partitioned);
    }
}
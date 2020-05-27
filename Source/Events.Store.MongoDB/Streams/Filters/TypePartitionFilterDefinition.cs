// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    public class TypePartitionFilterDefinition : AbstractFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypePartitionFilterDefinition"/> class.
        /// </summary>
        /// <param name="types">The event artifact types.</param>
        public TypePartitionFilterDefinition(IEnumerable<Guid> types)
        {
            Types = types;
        }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactId"> types</see> that this definition filters on.
        /// </summary>
        public IEnumerable<Guid> Types { get; set; }

        /// <inheritdoc/>
        public override IFilterDefinition AsRuntimeRepresentation(Guid streamId, bool partitioned, bool @public) =>
            new TypeFilterWithEventSourcePartitionDefinition(
                StreamId.EventLog,
                streamId,
                Types.Select(_ => new ArtifactId { Value = _ }),
                partitioned);
    }
}

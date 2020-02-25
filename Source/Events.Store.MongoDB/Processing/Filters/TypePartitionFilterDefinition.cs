// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Processing.Filters;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    public class TypePartitionFilterDefinition
    {
        /// <summary>
        /// Gets or sets the target stream.
        /// </summary>
        [BsonId]
        public Guid TargetStream { get; set; }

        /// <summary>
        /// Gets or sets the source stream.
        /// </summary>
        public Guid SourceStream { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Artifacts.ArtifactId"> types</see> that this definition filters on.
        /// </summary>
        public IEnumerable<Guid> Types { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this type filter is partitioned.
        /// </summary>
        public bool Partitioned { get; set; }
    }
}
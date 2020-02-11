// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventLog
{
    /// <summary>
    /// Represents event sourcing specific aggregate root and event source metadata.
    /// </summary>
    public class AggregateMetadata
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not this event was applied by an aggregate root to an event source.
        /// </summary>
        public bool WasAppliedByAggregate { get; set; }

        /// <summary>
        /// Gets or sets the event source id.
        /// </summary>
        public Guid EventSourceId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactId"/> of the <see cref="Artifact"/> identitying the type of the aggregate root.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactGeneration"/> of the <see cref="Artifact"/> identifying the type of the aggregate root.
        /// </summary>
        public int TypeGeneration { get; set; }

        /// <summary>
        /// Gets or sets the aggregate root version.
        /// </summary>
        public uint Version { get; set; }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Execution;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represent an Event that was applied to an Event Source by an aggregate root and is committed to the Event Store.
    /// </summary>
    public class CommittedAggregateEvent : CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedAggregateEvent"/> class.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root <see cref="Artifact" />.</param>
        /// <param name="aggregateRootVersion">The version of the aggregate root that applied the Event.</param>
        /// <param name="eventLogSequenceNumber">The version of the Event Log the Event was committed to.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.</param>
        /// <param name="eventSource">The Event Source that the Event was applied to.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext" />.</param>
        /// <param name="type">The <see cref="Artifact"/> representing the type of the Event.</param>
        /// <param name="isPublic">Whether this Event is public.</param>
        /// <param name="content">The content of the Event represented as a JSON-encoded <see cref="string"/>.</param>
        public CommittedAggregateEvent(
            Artifact aggregateRoot,
            AggregateRootVersion aggregateRootVersion,
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSource,
            ExecutionContext executionContext,
            Artifact type,
            bool isPublic,
            string content)
            : base(eventLogSequenceNumber, occurred, eventSource, executionContext, type, isPublic, content)
        {
            AggregateRoot = aggregateRoot;
            AggregateRootVersion = aggregateRootVersion;
        }

        /// <summary>
        /// Gets the aggregate root that this Event was committed to.
        /// </summary>
        public Artifact AggregateRoot { get; }

        /// <summary>
        /// Gets the version of the aggregate root after the Event was applied.
        /// </summary>
        public AggregateRootVersion AggregateRootVersion { get; }
    }
}

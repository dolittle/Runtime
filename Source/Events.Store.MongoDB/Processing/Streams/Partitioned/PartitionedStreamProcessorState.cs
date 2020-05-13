// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.Partitioned
{
    /// <summary>
    /// Represents the state of a <see cref="Runtime.Events.Processing.Streams.Partitioned.ScopedStreamProcessor" />.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class PartitionedStreamProcessorState : AbstractStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionedStreamProcessorState"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="failingPartitions">The states of the failing partitions.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        public PartitionedStreamProcessorState(Guid scopeId, Guid eventProcessorId, Guid sourceStreamId, ulong position, IDictionary<string, FailingPartitionState> failingPartitions, DateTimeOffset lastSuccessfullyProcessed)
            : base(scopeId, eventProcessorId, sourceStreamId, position, true, lastSuccessfullyProcessed, typeof(PartitionedStreamProcessorState).Name)
        {
            FailingPartitions = failingPartitions;
        }

        /// <summary>
        /// Gets or sets the failing partitions.
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public IDictionary<string, FailingPartitionState> FailingPartitions { get; set; }
    }
}

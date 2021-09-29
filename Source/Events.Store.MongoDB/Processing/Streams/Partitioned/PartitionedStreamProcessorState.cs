// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using runtime = Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.Partitioned
{
    /// <summary>
    /// Represents the state of a <see cref="runtime.Partitioned.ScopedStreamProcessor" />.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class PartitionedStreamProcessorState : AbstractStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionedStreamProcessorState"/> class.
        /// </summary>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="failingPartitions">The states of the failing partitions.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        public PartitionedStreamProcessorState(Guid eventProcessorId, Guid sourceStreamId, ulong position, IDictionary<string, FailingPartitionState> failingPartitions, DateTime lastSuccessfullyProcessed)
            : base(eventProcessorId, sourceStreamId, position, lastSuccessfullyProcessed)
        {
            FailingPartitions = failingPartitions;
        }

        /// <summary>
        /// Gets or sets the failing partitions.
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public IDictionary<string, FailingPartitionState> FailingPartitions { get; set; }

        /// <inheritdoc/>
        public override IStreamProcessorState ToRuntimeRepresentation() =>
            new runtime.Partitioned.StreamProcessorState(
                Position,
                FailingPartitions.ToDictionary(_ => new PartitionId(Guid.Parse(_.Key)), _ => _.Value.ToRuntimeRepresentation()),
                LastSuccessfullyProcessed);
    }
}

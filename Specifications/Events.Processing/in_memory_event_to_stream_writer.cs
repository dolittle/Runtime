// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    public class in_memory_event_to_stream_writer : IWriteEventsToStreams
    {
        readonly IDictionary<StreamId, IDictionary<PartitionId, IList<CommittedEvent>>> streams = new Dictionary<StreamId, IDictionary<PartitionId, IList<CommittedEvent>>>();

        public Task Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken = default)
        {
            if (streams.ContainsKey(streamId))
            {
                var stream = streams[streamId];
                if (stream.ContainsKey(partitionId))
                {
                    var events = stream[partitionId];
                    events.Add(@event);
                    stream[partitionId] = events;
                    streams[streamId] = stream;
                }
                else
                {
                    stream.Add(partitionId, new CommittedEvent[] { @event });
                    streams[streamId] = stream;
                }
            }
            else
            {
                streams.Add(streamId, new Dictionary<PartitionId, IList<CommittedEvent>>
                {
                    { partitionId, new CommittedEvent[] { @event } }
                });
            }

            return Task.FromResult(true);
        }
    }
}
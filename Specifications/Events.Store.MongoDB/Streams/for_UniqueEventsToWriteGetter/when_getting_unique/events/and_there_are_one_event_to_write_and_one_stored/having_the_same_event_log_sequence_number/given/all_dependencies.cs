// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.events.and_there_are_one_event_to_write_and_one_stored.having_the_same_event_log_sequence_number.given;

public class all_dependencies : for_UniqueEventsToWriteGetter.given.all_dependencies
{
    protected static EventLogSequenceNumber event_log_sequence_number;
    protected static Artifact artifact;
    protected static EventSourceId event_source;
    protected static PartitionId partition;
    
    Establish context = () =>
    {
        event_log_sequence_number = 0;
        artifact = new Artifact("7a227d7b-0189-43a1-bec7-fcc696091eca", 0);
        event_source = "some event source";
        partition = "partition";
    };
}
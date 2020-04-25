// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher.when_finding_next_position
{
    public class in_all_stream : given.all_dependencies
    {
        Because of = () => events_from_streams_fetcher.FindNext(ScopeId.Default, StreamId.AllStreamId, PartitionId.NotSet, 0U, CancellationToken.None).GetAwaiter().GetResult();

        It should_use_events_from_event_log_fetcher = () => events_from_event_log_fetcher.Verify(_ => _.FindNext(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}
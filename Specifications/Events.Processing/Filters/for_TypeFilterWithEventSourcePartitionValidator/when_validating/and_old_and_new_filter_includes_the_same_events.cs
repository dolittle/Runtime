// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartitionValidator.when_validating
{
    public class and_old_and_new_filter_includes_the_same_events : given.all_dependencies
    {
        static StreamEvent @event;
        static Exception exception;

        Establish context = () =>
        {
            @event = given.stream_event.single();
            streams_metadata
                .Setup(_ => _.GetLastProcessedEventLogSequenceNumber(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<EventLogSequenceNumber>(1));

            event_types_fetcher
                .Setup(_ => _.FetchTypesInRange(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Artifact[] { @event.Event.Type }.AsEnumerable()));
            events_fetcher
                .Setup(_ => _.FetchRange(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new StreamEvent[] { @event }.AsEnumerable()));
            filter_processor
                .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<EventProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IFilterResult>(new SucceededFilteringResult(true, PartitionId.NotSet)));
        };

        Because of = () => exception = Catch.Exception(() => validator.Validate(filter_processor.Object).GetAwaiter().GetResult());

        It should_not_fail_validation = () => exception.ShouldBeNull();
    }
}
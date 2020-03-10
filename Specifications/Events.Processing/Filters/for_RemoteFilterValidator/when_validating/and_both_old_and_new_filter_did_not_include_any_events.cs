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

namespace Dolittle.Runtime.Events.Processing.Filters.for_RemoteFilterValidator.when_validating
{
    public class and_both_old_and_new_filter_did_not_include_any_events : given.all_dependencies
    {
        static Exception exception;

        Establish context = () =>
        {
            streams_metadata
                .Setup(_ => _.GetLastProcessedEventLogSequenceNumber(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<EventLogSequenceNumber>(0));

            event_types_fetcher
                .Setup(_ => _.FetchTypesInRange(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Enumerable.Empty<Artifact>()));
            events_fetcher
                .Setup(_ => _.FetchRange(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Enumerable.Empty<StreamEvent>()));
        };

        Because of = () => exception = Catch.Exception(() => validator.Validate(filter_processor.Object).GetAwaiter().GetResult());

        It should_not_fail_validation = () => exception.ShouldBeNull();
    }
}
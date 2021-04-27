// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_filtering_fails : given.all_dependencies
    {
        static FilterValidationResult result;

        Establish context = () =>
        {
            var @event = committed_events.single(0);
            stream_processor_states
                .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
                .Returns(Task.FromResult<Try<IStreamProcessorState>>((true, new StreamProcessorState(1, DateTimeOffset.UtcNow))));
            add_event_to_event_log(@event);
            filter_processor
                .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), event_processor_id, cancellation_token))
                .Returns(Task.FromResult<IFilterResult>(new FailedFiltering("something went wrong")));
        };

        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, cancellation_token).GetAwaiter().GetResult();
        It should_fail_validation = () => result.Succeeded.ShouldBeFalse();
    }
}
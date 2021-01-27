// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_old_filter_included_more_events_than_new_filter : given.all_dependencies
    {
        static FilterValidationResult result;
        static int num_times_filtered = 0;

        Establish context = () =>
        {
            stream_processor_states
                .Setup(_ => _.TryGetFor(Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<IStreamProcessorState>>((true, new StreamProcessorState(1, DateTimeOffset.UtcNow))));
            add_event_to_event_log(3);
            add_event_to_filtered_stream(3);
            filter_processor
                .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<EventProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(() => num_times_filtered++ > 1 ? Task.FromResult<IFilterResult>(new SuccessfulFiltering(false)) : Task.FromResult<IFilterResult>(new SuccessfulFiltering(true)));
        };

        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, CancellationToken.None).GetAwaiter().GetResult();

        It should_fail_validation = () => result.Succeeded.ShouldBeFalse();
    }
}
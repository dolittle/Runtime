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
    public class and_both_old_and_new_filter_did_not_include_any_events : given.all_dependencies
    {
        static FilterValidationResult result;

        Establish context = () =>
        {
            _ = stream_processor_states
                .Setup(_ => _.TryGetFor(Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<IStreamProcessorState>>((true, new StreamProcessorState(1, DateTimeOffset.UtcNow))));
            add_event_to_event_log(1);
            filter_processor
                .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<EventProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IFilterResult>(new SuccessfulFiltering(false)));
        };

        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, CancellationToken.None).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
    }
}
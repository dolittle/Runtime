// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_both_old_and_new_filter_did_not_include_any_events : given.all_dependencies
    {
        Establish context = () =>
        {
            _ = stream_processor_states
                .Setup(_ => _.TryGetFor(Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<IStreamProcessorState>.Succeeded(new StreamProcessorState(1, DateTimeOffset.UtcNow))));
            add_event_to_event_log(1);
            filter_processor
                .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), event_processor_id, cancellation_token))
                .Returns(Task.FromResult<IFilterResult>(new SuccessfulFiltering(false)));
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, 1, cancellation_token).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
    }
}
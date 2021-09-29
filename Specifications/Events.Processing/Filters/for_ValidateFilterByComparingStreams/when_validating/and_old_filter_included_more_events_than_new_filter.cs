// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_old_filter_included_more_events_than_new_filter : given.all_dependencies
    {
        static int num_times_filtered = 0;

        Establish context = () =>
        {
            add_event_to_event_log(3);
            add_event_to_filtered_stream(3);
            filter_processor
                .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), event_processor_id, cancellation_token))
                .Returns(() => num_times_filtered++ > 1 ? Task.FromResult<IFilterResult>(new SuccessfulFiltering(false)) : Task.FromResult<IFilterResult>(new SuccessfulFiltering(true)));
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, 1, cancellation_token).GetAwaiter().GetResult();

        It should_fail_validation = () => result.Success.ShouldBeFalse();
    }
}
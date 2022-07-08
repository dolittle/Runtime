// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating;

public class and_filtering_fails : given.all_dependencies
{
    Establish context = () =>
    {
        var @event = committed_events.single(0);
        add_event_to_event_log(@event);
        filter_processor
            .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), event_processor_id, Moq.It.IsAny<ExecutionContext>(), cancellation_token))
            .Returns(Task.FromResult<IFilterResult>(new FailedFiltering("something went wrong")));
    };

    static FilterValidationResult result;
    Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, 1, cancellation_token).GetAwaiter().GetResult();

    It should_fail_validation = () => result.Success.ShouldBeFalse();
}
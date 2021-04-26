// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating
{
    public class and_filter_and_persisted_filter_definition_does_not_have_the_same_partitioned_value : given.all_dependencies
    {
        protected static TypeFilterWithEventSourcePartitionDefinition new_filter_definition;

        Establish context = () =>
        {
            new_filter_definition = new TypeFilterWithEventSourcePartitionDefinition(
                source_stream,
                target_stream,
                new[] {
                    filter_definition_event_type_one.Id,
                    filter_definition_event_type_two.Id,
                    filter_definition_event_type_three.Id,
                },
                true);
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(new_filter_definition, filter_processor, CancellationToken.None).GetAwaiter().GetResult();

        It should_fail_validation = () => result.Succeeded.ShouldBeFalse();
        It should_not_get_the_stream_processor_state = () => stream_processor_states.VerifyNoOtherCalls();
        It should_not_fetch_event_types = () => events_fetchers.VerifyNoOtherCalls();
    }
}
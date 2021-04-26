// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Machine.Specifications;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating
{
    public class and_filter_has_not_processed_any_events : given.all_dependencies
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
                },
                false);

            stream_processor_states
                .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
                .Returns(Task.FromResult<Try<IStreamProcessorState>>((true, StreamProcessorState.New)));
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(new_filter_definition, filter_processor, CancellationToken.None).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
        It should_not_fetch_event_types = () => events_fetchers.VerifyNoOtherCalls();
    }
}
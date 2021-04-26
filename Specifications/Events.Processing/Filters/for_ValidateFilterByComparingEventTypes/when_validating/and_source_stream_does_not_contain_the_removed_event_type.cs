// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Machine.Specifications;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using System;
using Dolittle.Runtime.Events.Store.Streams;
using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating
{
    public class and_source_stream_does_not_contain_the_removed_event_type : given.all_dependencies
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
                .Returns(Task.FromResult<Try<IStreamProcessorState>>(new StreamProcessorState(10, DateTimeOffset.Now)));

            types_fetcher
                .Setup(_ => _.FetchInRange(new StreamPositionRange(0, 10), cancellation_token))
                .Returns(Task.FromResult<ISet<Artifact>>(new HashSet<Artifact>(new[] { filter_definition_event_type_one, event_type_four })));
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(new_filter_definition, filter_processor, CancellationToken.None).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating
{
    public class and_source_stream_does_not_contain_the_added_event_type : given.all_dependencies
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
                    event_type_four.Id,
                },
                false);

            types_fetcher
                .Setup(_ => _.FetchInRange(new StreamPositionRange(0, 32), cancellation_token))
                .Returns(Task.FromResult<ISet<Artifact>>(new HashSet<Artifact>(new[] { filter_definition_event_type_one, filter_definition_event_type_three })));
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(new_filter_definition, filter_processor, 32, CancellationToken.None).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
    }
}

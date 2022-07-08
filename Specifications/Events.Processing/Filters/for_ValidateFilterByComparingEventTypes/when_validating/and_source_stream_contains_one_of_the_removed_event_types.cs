// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating;

public class and_source_stream_contains_one_of_the_removed_event_types : given.all_dependencies
{
    protected static TypeFilterWithEventSourcePartitionDefinition new_filter_definition;

    Establish context = () =>
    {
        new_filter_definition = new TypeFilterWithEventSourcePartitionDefinition(
            source_stream,
            target_stream,
            new[] {
                filter_definition_event_type_one.Id,
            },
            false);

        types_fetcher
            .Setup(_ => _.FetchInRange(new StreamPositionRange(0, 7), cancellation_token))
            .Returns(Task.FromResult<ISet<Artifact>>(new HashSet<Artifact>(new[] { filter_definition_event_type_one, filter_definition_event_type_three, event_type_four })));
    };

    static FilterValidationResult result;
    Because of = () => result = validator.Validate(new_filter_definition, filter_processor, 7, CancellationToken.None).GetAwaiter().GetResult();

    It should_fail_validation = () => result.Success.ShouldBeFalse();
}
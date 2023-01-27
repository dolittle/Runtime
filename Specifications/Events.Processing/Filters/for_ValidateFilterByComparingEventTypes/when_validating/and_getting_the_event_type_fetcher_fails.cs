// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating;

public class and_getting_the_event_type_fetcher_fails : given.all_dependencies
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

        events_fetchers
            .Setup(_ => _.GetTypeFetcherFor(scope_id, new EventLogStreamDefinition(), cancellation_token))
            .Returns(Task.FromException<ICanFetchEventTypesFromStream>(new Exception()));
    };

    static FilterValidationResult result;
    Because of = () => result = validator.Validate(new_filter_definition, filter_processor, 87, CancellationToken.None).GetAwaiter().GetResult();

    It should_fail_validation = () => result.Success.Should().BeFalse();
}
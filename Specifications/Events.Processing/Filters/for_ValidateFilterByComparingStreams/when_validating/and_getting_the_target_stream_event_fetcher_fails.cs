// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating;

public class and_getting_the_target_stream_event_fetcher_fails : given.all_dependencies
{
    Establish context = () =>
    {
        events_fetchers
            .Setup(_ => _.GetRangeFetcherFor(scope_id, new StreamDefinition(filter_definition), cancellation_token))
            .Returns(Task.FromException<ICanFetchRangeOfEventsFromStream>(new Exception()));
    };

    static FilterValidationResult result;
    Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, 10, CancellationToken.None).GetAwaiter().GetResult();

    It should_fail_validation = () => result.Success.Should().BeFalse();
}
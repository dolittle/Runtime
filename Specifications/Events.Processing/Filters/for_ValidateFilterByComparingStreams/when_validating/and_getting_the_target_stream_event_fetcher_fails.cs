// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_getting_the_target_stream_event_fetcher_fails : given.all_dependencies
    {
        static FilterValidationResult result;

        Establish context = () =>
        {
            stream_processor_states
                .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
                .Returns(Task.FromResult<Try<IStreamProcessorState>>(new StreamProcessorState(10, DateTimeOffset.Now)));

            events_fetchers
                .Setup(_ => _.GetRangeFetcherFor(scope_id, new StreamDefinition(filter_definition), cancellation_token))
                .Returns(Task.FromException<ICanFetchRangeOfEventsFromStream>(new Exception()));
        };

        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, CancellationToken.None).GetAwaiter().GetResult();
        It should_fail_validation = () => result.Succeeded.ShouldBeFalse();
    }
}

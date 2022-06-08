// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_fetching_the_source_stream_events_fails : given.all_dependencies
    {
        Establish context = () =>
        {
            events_from_event_log_fetcher
                .Setup(_ => _.FetchRange(new StreamPositionRange(StreamPosition.Start, 10), cancellation_token))
                .Throws(new Exception());
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, 10, CancellationToken.None).GetAwaiter().GetResult();

        It should_fail_validation = () => result.Success.ShouldBeFalse();
    }
}

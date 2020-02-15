// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range
{
    public class and_stream_has_no_events : given.all_dependencies
    {
        static EventTypesFromStreamsFetcher event_types_from_streams;
        static StreamId stream;
        static IEnumerable<Artifact> result;

        Establish context = () =>
        {
            event_types_from_streams = new EventTypesFromStreamsFetcher(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream = Guid.NewGuid();
        };

        Because of = () => result = event_types_from_streams.FetchTypesInRange(stream, 0U, 0U).GetAwaiter().GetResult();

        It should_return_empty_list = () => result.ShouldBeEmpty();
    }
}
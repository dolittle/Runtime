// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range_and_partition
{
    public class and_stream_has_no_events : given.all_dependencies
    {
        static StreamId stream;
        static IEnumerable<Artifact> result;

        Establish context = () => stream = Guid.NewGuid();

        Because of = () => result = event_types_from_streams.FetchInRange(Moq.It.IsAny<ScopeId>(), stream, new StreamPositionRange(0U, 1), CancellationToken.None).GetAwaiter().GetResult();

        It should_return_empty_list = () => result.ShouldBeEmpty();
    }
}
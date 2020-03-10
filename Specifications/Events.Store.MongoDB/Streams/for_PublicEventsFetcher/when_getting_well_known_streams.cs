// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsFetcher
{
    public class when_getting_well_known_streams : given.all_dependencies
    {
        It should_only_have_public_events_stream = () => fetcher.WellKnownStreams.ShouldContainOnly(StreamId.PublicEventsId);
    }
}
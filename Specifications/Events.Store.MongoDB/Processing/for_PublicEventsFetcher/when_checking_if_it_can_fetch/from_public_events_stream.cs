// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_PublicEventsFetcher.when_checking_if_it_can_fetch
{
    public class from_public_events_stream : given.all_dependencies
    {
        static bool result;

        Because of = () => fetcher.CanFetchFromStream(StreamId.PublicEventsId);
        It should_be_able_to_fetch = () => result.ShouldBeTrue();
    }
}
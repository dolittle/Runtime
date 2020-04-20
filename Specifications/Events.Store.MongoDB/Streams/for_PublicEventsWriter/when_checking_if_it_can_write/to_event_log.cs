// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsWriter.when_checking_if_it_can_write
{
    public class to_event_log : given.all_dependencies
    {
        static bool result;

        Because of = () => result = public_events_writer.CanWriteToStream(StreamId.AllStreamId);
        It should_not_be_able_to_write = () => result.ShouldBeFalse();
    }
}
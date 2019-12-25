// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedEventStream.given
{
    public abstract class an_empty_committed_event_stream
    {
        protected static CommittedEvents event_stream;
        protected static EventSourceId event_source_id;

        Establish context = () =>
                {
                    event_source_id = Guid.NewGuid();
                    event_stream = new CommittedEvents(event_source_id);
                };
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Specs.for_UncommittedEventStream.given
{
    public abstract class an_empty_uncommitted_event_stream
    {
        protected static UncommittedEvents event_stream;
        protected static EventSourceId event_source_id;
        protected static Mock<IEventSource> event_source;

        Establish context = () =>
                {
                    event_source_id = Guid.NewGuid();
                    event_source = new Mock<IEventSource>();
                    event_source.SetupGet(e => e.EventSourceId).Returns(event_source_id);

                    event_stream = new UncommittedEvents(event_source.Object);
                };
    }
}
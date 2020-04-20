// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.EventHorizon.for_EventHorizonEventProcessor
{
    public class when_creating_event
    {
        static StreamPosition position;
        static EventHorizonEventMetadata metadata;
        static BsonDocument content;
        static EventHorizonEvent @event;

        Establish context = () =>
        {
            position = 0;
            metadata = new EventHorizonEventMetadata(
                DateTimeOffset.Now,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                Guid.NewGuid(),
                0);
            content = new BsonDocument();
        };

        Because of = () => @event = new EventHorizonEvent(position, metadata, content);

        It should_have_the_correct_position = () => @event.StreamPosition.ShouldEqual(position.Value);
        It should_have_the_correct_metadata = () => @event.Metadata.ShouldEqual(metadata);
        It should_have_the_correct_content = () => @event.Content.ShouldEqual(content);
    }
}
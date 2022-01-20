// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorId;

public class when_creating_id
{
    static ScopeId scope_id;
    static EventProcessorId event_processor_id;
    static StreamId source_stream_id;
    static StreamProcessorId id;

    Establish context = () =>
    {
        scope_id = Guid.NewGuid();
        event_processor_id = Guid.NewGuid();
        source_stream_id = Guid.NewGuid();
    };

    Because of = () => id = new StreamProcessorId(scope_id, event_processor_id, source_stream_id);

    It should_have_the_correct_scope_id = () => id.ScopeId.ShouldEqual(scope_id);
    It should_have_the_correct_event_processor_id = () => id.EventProcessorId.ShouldEqual(event_processor_id);
    It should_have_the_correct_source_stream_id = () => id.SourceStreamId.ShouldEqual(source_stream_id);
}
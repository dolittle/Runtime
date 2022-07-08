// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorState;

public class when_creating_state
{
    static StreamPosition stream_position;
    static StreamProcessorState state;

    Establish context = () =>
    {
        stream_position = 0;
    };

    Because of = () => state = new StreamProcessorState(stream_position, DateTimeOffset.UtcNow);

    It should_have_the_correct_stream_position = () => state.Position.ShouldEqual(stream_position);
}
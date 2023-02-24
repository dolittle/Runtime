// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorState;

public class when_creating_state
{
    static ProcessingPosition processing_position;
    static StreamProcessorState state;

    Establish context = () =>
    {
        processing_position = ProcessingPosition.Initial;
    };

    Because of = () => state = new StreamProcessorState(processing_position, DateTimeOffset.UtcNow);

    It should_have_the_correct_stream_position = () => state.Position.ShouldEqual(processing_position);
}
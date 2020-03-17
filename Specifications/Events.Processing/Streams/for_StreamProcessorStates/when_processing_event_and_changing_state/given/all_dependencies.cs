// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorStates.when_processing_event_and_changing_state.given
{
    public class all_dependencies : for_StreamProcessorStates.given.all_dependencies
    {
        protected static Mock<IEventProcessor> event_processor;

        Establish context = () =>
        {
            event_processor = new Mock<IEventProcessor>();
        };
    }
}
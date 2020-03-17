// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorStates.when_getting_stored_state_for
{
    public class a_stream_processor : given.all_dependencies
    {
        static StreamProcessorId stream_processor_id;

        Establish context = () => stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid());

        Because of = () => stream_processor_states.GetStoredStateFor(stream_processor_id, default).GetAwaiter().GetResult();

        It should_try_to_get_state_for_stream_processor = () => stream_processor_state_repository.Verify(_ => _.GetOrAddNew(stream_processor_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}
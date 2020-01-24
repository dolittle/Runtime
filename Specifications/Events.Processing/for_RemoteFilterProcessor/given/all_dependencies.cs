// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteFilterProcessor.given
{
    public class all_dependencies
    {
        protected static readonly CommittedEventEnvelope an_event = Processing.given.a_committed_event_envelope;

        protected static EventProcessorId event_processor_id;
        protected static StreamId target_stream_id;
        protected static Moq.Mock<IRemoteFilterService> remote_filter_mock;
        protected static Moq.Mock<IWriteEventToStream> event_to_stream_writer_mock;

        Establish context = () =>
        {
            event_processor_id = Guid.NewGuid();
            target_stream_id = Guid.NewGuid();
            remote_filter_mock = new Moq.Mock<IRemoteFilterService>();
            event_to_stream_writer_mock = new Moq.Mock<IWriteEventToStream>();
        };
    }
}
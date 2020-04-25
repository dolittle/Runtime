// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessor
{
    public class when_creating_a_stream_processor : given.all_dependencies
    {
        static StreamProcessor stream_processor;

        Because of = () => stream_processor = new StreamProcessor(tenant_id, source_stream_id, event_processor.Object, stream_processor_states, next_event_fetcher.Object, stream_processors.Object, Moq.Mock.Of<ILogger>(), CancellationToken.None);

        It should_have_the_correct_event_processor_id = () => stream_processor.EventProcessorId.ShouldEqual(event_processor.Object.Identifier);
        It should_have_the_correct_key = () => stream_processor.Identifier.ShouldEqual(new StreamProcessorId(scope_id, event_processor.Object.Identifier, source_stream_id));
        It should_have_the_correct_initial_position = () => stream_processor.CurrentState.Position.ShouldEqual(StreamPosition.Start);
        It should_not_have_any_failing_partitions = () => stream_processor.CurrentState.FailingPartitions.Count.ShouldEqual(0);
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessors.when_registering
{
    public class and_processor_key_has_not_been_registered_before : given.all_dependencies
    {
        static EventProcessorId event_processor_id;
        static StreamId source_stream_id;
        static ScopeId scope_id;
        static IStreamDefinition stream_definition;
        static Moq.Mock<IEventProcessor> event_processor;

        Establish context = () =>
        {
            event_processor_id = Guid.NewGuid();
            source_stream_id = Guid.NewGuid();
            scope_id = Guid.NewGuid();
            stream_definition = new StreamDefinition(new FilterDefinition(source_stream_id, event_processor_id.Value, false));
            event_processor = new Moq.Mock<IEventProcessor>();
            event_processor.SetupGet(_ => _.Identifier).Returns(event_processor_id);
        };

        static Try<StreamProcessor> result;
        Because of = () => result = stream_processors.TryCreateAndRegister(scope_id, event_processor_id, stream_definition, () => event_processor.Object, CancellationToken.None);

        It should_register_stream_processor = () => result.Success.ShouldBeTrue();
        It should_return_the_registered_stream_processor = () => result.Result.ShouldNotBeNull();
    }
}
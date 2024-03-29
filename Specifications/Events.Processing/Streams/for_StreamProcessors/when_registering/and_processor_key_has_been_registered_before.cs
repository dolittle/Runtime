// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessors.when_registering;

public class and_processor_key_has_been_registered_before : given.all_dependencies
{
    static EventProcessorId event_processor_id;
    static EventProcessorKind event_processor_kind;
    static StreamId source_stream_id;
    static ScopeId scope_id;
    static IStreamDefinition stream_definition;
    static Moq.Mock<IEventProcessor> event_processor;

    Establish context = () =>
    {
        event_processor_id = Guid.NewGuid();
        event_processor_kind = "test";
        source_stream_id = Guid.NewGuid();
        scope_id = Guid.NewGuid();
        stream_definition = new StreamDefinition(new FilterDefinition(source_stream_id, event_processor_id.Value, false));
        event_processor = new Moq.Mock<IEventProcessor>();
        event_processor.SetupGet(_ => _.Identifier).Returns(event_processor_id);
    };

    static Try<StreamProcessor> first_registration_result;
    static Try<StreamProcessor> second_registration_result;

    Because of = () =>
    {
        first_registration_result = stream_processors.TryCreateAndRegister(scope_id, event_processor_id,  event_processor_kind, stream_definition, tenant_id => event_processor.Object, execution_contexts.create(), CancellationToken.None);
        second_registration_result = stream_processors.TryCreateAndRegister(scope_id, event_processor_id, event_processor_kind, stream_definition, tenant_id => event_processor.Object, execution_contexts.create(), CancellationToken.None);
    };

    It should_have_registered_the_first_time = () => first_registration_result.Success.ShouldBeTrue();
    It should_not_have_registered_the_second_time = () => second_registration_result.Success.ShouldBeFalse();
}

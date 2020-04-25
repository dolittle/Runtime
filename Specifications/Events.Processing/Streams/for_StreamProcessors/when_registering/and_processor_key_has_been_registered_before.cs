// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessors.when_registering
{
    [Ignore("Not implemented")]
    public class and_processor_key_has_been_registered_before : given.all_dependencies
    {
        static readonly TenantId tenant = Guid.NewGuid();
        static readonly EventProcessorId event_processor_id = Guid.NewGuid();
        static readonly StreamId source_stream_id = Guid.NewGuid();
        static Moq.Mock<IEventProcessor> event_processor_mock;
        static IStreamProcessors stream_processors;
        static Exception exception;

        Establish context = () =>
        {
            execution_context_manager_mock.SetupGet(_ => _.Current).Returns(execution_contexts.create());
            event_processor_mock = new Moq.Mock<IEventProcessor>();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(event_processor_id);
            stream_processors = new StreamProcessors(
                stream_processor_state_repository,
                execution_context_manager_mock.Object,
                Moq.Mock.Of<ILogger>());

            stream_processors.Register(event_processor_mock.Object, next_event_fetcher_mock.Object, source_stream_id, CancellationToken.None);
        };

        Because of = () => exception = Catch.Exception(() => stream_processors.Register(event_processor_mock.Object, next_event_fetcher_mock.Object, source_stream_id, CancellationToken.None));

        It should_throw_an_exception = () => exception.ShouldNotBeNull();

        It should_throw_StreamProcessorKeyAlreadyRegistered = () => exception.ShouldBeOfExactType<StreamProcessorKeyAlreadyRegistered>();
    }
}
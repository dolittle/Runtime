// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Security;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessors.when_registering
{
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
            execution_context_manager_mock.SetupGet(_ => _.Current).Returns(new Execution.ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                tenant,
                "env",
                Guid.NewGuid(),
                Claims.Empty,
                CultureInfo.CurrentCulture));
            event_processor_mock = new Moq.Mock<IEventProcessor>();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(event_processor_id);
            stream_processors = new StreamProcessors(
                stream_processor_state_repository,
                next_event_fetcher_mock.Object,
                execution_context_manager_mock.Object,
                Moq.Mock.Of<ILogger>());

            stream_processors.Register(event_processor_mock.Object, source_stream_id);
        };

        Because of = () => exception = Catch.Exception(() => stream_processors.Register(event_processor_mock.Object, source_stream_id));

        It should_throw_an_exception = () => exception.ShouldNotBeNull();

        It should_throw_StreamProcessorKeyAlreadyRegistered = () => exception.ShouldBeOfExactType<StreamProcessorKeyAlreadyRegistered>();
    }
}
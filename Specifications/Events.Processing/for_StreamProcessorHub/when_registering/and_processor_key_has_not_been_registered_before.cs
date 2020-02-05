// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dolittle.Logging;
using Dolittle.Security;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessorHub.when_registering
{
    public class and_processor_key_has_not_been_registered_before : given.all_dependencies
    {
        static readonly TenantId tenant = Guid.NewGuid();
        static readonly EventProcessorId event_processor_id = Guid.NewGuid();
        static readonly StreamId source_stream_id = Guid.NewGuid();
        static Moq.Mock<IEventProcessor> event_processor_mock;
        static IStreamProcessorHub stream_processor_hub;
        static IEnumerable<StreamProcessor> registered_stream_processors;

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
            stream_processor_hub = new StreamProcessorHub(
                stream_processor_state_repository,
                next_event_fetcher_mock.Object,
                execution_context_manager_mock.Object,
                Moq.Mock.Of<ILogger>());
        };

        Because of = () =>
        {
            stream_processor_hub.Register(event_processor_mock.Object, source_stream_id);
            registered_stream_processors = stream_processor_hub.StreamProcessors;
        };

        It should_have_registered_one_stream_processor = () => registered_stream_processors.Count().ShouldEqual(1);

        It should_register_a_stream_processor_with_the_correct_key = () => registered_stream_processors.First().Identifier.ShouldEqual(new StreamProcessorId(event_processor_id, source_stream_id));
    }
}
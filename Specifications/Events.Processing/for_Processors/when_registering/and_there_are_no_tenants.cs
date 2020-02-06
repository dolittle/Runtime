// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Logging;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_Processors.when_registering
{
    public class and_there_are_no_tenants : given.all_dependencies
    {
        static readonly EventProcessorId event_processor_id = Guid.NewGuid();
        static readonly StreamId stream_id = Guid.NewGuid();

        static IProcessors processors;

        Establish context = () =>
        {
            tenants_mock.SetupGet(_ => _.All).Returns(Enumerable.Empty<TenantId>());
            processors = new Processors(
                execution_context_manager_mock.Object,
                tenants_mock.Object,
                Processing.given.a_remote_processor_service(new SucceededProcessingResult()),
                get_stream_processor_hub_mock.Object,
                Moq.Mock.Of<ILogger>());
        };

        Because of = () => processors.Register(event_processor_id, stream_id);

        It should_set_execution_context_for_tenant = () => execution_context_manager_mock.VerifyNoOtherCalls();
        It should_register_stream_processor_hub_with_correct_stream_id = () => stream_processor_hub_mock.VerifyNoOtherCalls();
    }
}
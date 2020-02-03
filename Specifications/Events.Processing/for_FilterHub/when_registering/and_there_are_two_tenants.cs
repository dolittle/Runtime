// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_FilterHub.when_registering
{
    public class and_there_are_two_tenants : given.all_dependencies
    {
        static readonly TenantId tenant1 = Guid.NewGuid();
        static readonly TenantId tenant2 = Guid.NewGuid();
        static readonly FilterId filter_id = Guid.NewGuid();
        static readonly StreamId stream_id = Guid.NewGuid();

        static IFilterHub filter_hub;

        Establish context = () =>
        {
            tenants_mock.SetupGet(_ => _.All).Returns(new TenantId[] { tenant1, tenant2 });
            filter_hub = new FilterHub(
                tenants_mock.Object,
                execution_context_manager_mock.Object,
                Processing.given.a_remote_filter_service(new SucceededFilteringResult(true, 0)),
                get_stream_processor_hub_mock.Object,
                get_event_to_stream_writer_mock.Object,
                Moq.Mock.Of<ILogger>());
        };

        Because of = () => filter_hub.Register(filter_id, stream_id);

        It should_set_execution_context_for_tenant1 = () => execution_context_manager_mock.Verify(_ => _.CurrentFor(tenant1, Moq.It.IsAny<string>(), Moq.It.IsAny<int>(), Moq.It.IsAny<string>()));
        It should_set_execution_context_for_tenant2 = () => execution_context_manager_mock.Verify(_ => _.CurrentFor(tenant2, Moq.It.IsAny<string>(), Moq.It.IsAny<int>(), Moq.It.IsAny<string>()));
        It should_register_stream_processor_hub_with_correct_stream_id = () => stream_processor_hub_mock.Verify(_ => _.Register(Moq.It.IsAny<IEventProcessor>(), StreamId.AllStreamId));
    }
}
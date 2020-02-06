// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Logging;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_Filters.when_registering
{
    public class and_there_are_no_tenants : given.all_dependencies
    {
        static readonly StreamId target_stream_id = Guid.NewGuid();
        static readonly StreamId source_stream_id = StreamId.AllStreamId;

        static IFilters filters;

        Establish context = () =>
        {
            tenants_mock.SetupGet(_ => _.All).Returns(Enumerable.Empty<TenantId>());
            filters = new Filters(
                tenants_mock.Object,
                execution_context_manager_mock.Object,
                Processing.given.a_remote_filter_service(new SucceededFilteringResult(true, PartitionId.NotSet)),
                get_stream_processor_hub_mock.Object,
                get_event_to_stream_writer_mock.Object,
                Moq.Mock.Of<ILogger>());
        };

        Because of = () => filters.Register(target_stream_id, source_stream_id);

        It should_set_execution_context_for_tenant = () => execution_context_manager_mock.VerifyNoOtherCalls();
        It should_register_stream_processor_hub_with_correct_stream_id = () => stream_processor_hub_mock.VerifyNoOtherCalls();
    }
}
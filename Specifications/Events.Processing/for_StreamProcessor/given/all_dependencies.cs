// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.DependencyInversion;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessor.given
{
    public class all_dependencies
    {
        protected static TenantId tenant_id;
        protected static StreamId source_stream_id;
        protected static FactoryFor<IStreamProcessorStateRepository> get_stream_processor_state_repository;
        protected static FactoryFor<IFetchNextEvent> get_next_event_fetcher;
        Establish context = () =>
        {
            tenant_id = Guid.NewGuid();
            source_stream_id = Guid.NewGuid();
            var in_memory_stream_processor_state_repository = new in_memory_stream_processor_state_repository();
            var get_stream_processor_state_repository_mock = new Mock<FactoryFor<IStreamProcessorStateRepository>>();
            get_stream_processor_state_repository_mock.Setup(_ => _.Invoke()).Returns(in_memory_stream_processor_state_repository);
            var get_next_event_fetcher_mock = new Mock<FactoryFor<IFetchNextEvent>>();
            get_next_event_fetcher_mock.Setup(_ => _.Invoke()).

            get_stream_processor_state_repository = get_stream_processor_state_repository_mock.Object;


        };
    }
}
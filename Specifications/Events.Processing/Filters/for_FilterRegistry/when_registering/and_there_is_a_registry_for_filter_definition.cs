// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterRegistry.when_registering
{
    public class and_there_is_a_registry_for_filter_definition : given.all_dependencies
    {
        static Moq.Mock<IFilterDefinitionRepositoryFor<RemoteFilterDefinition>> repository;
        static Moq.Mock<IFilterProcessor<RemoteFilterDefinition>> filter_processor;

        Establish context = () =>
        {
            repository = new Moq.Mock<IFilterDefinitionRepositoryFor<RemoteFilterDefinition>>();
            container.Setup(_ => _.Get<IFilterDefinitionRepositoryFor<RemoteFilterDefinition>>()).Returns(repository.Object);
            filter_processor = new Moq.Mock<IFilterProcessor<RemoteFilterDefinition>>();
            filter_processor.SetupGet(_ => _.Definition).Returns(new RemoteFilterDefinition(Guid.NewGuid(), Guid.NewGuid()));
        };

        Because of = () => filter_registry.Register(filter_processor.Object).GetAwaiter().GetResult();

        It should_fail_because_filter_for_stream_has_already_been_registered = () => repository.Verify(_ => _.PersistFilter(filter_processor.Object.Definition, Moq.It.IsAny<CancellationToken>()));
    }
}
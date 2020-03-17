// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterRegistry.when_removing_if_persisted
{
    public class and_there_is_a_filter_definition_remover : given.all_dependencies
    {
        static Moq.Mock<ICanRemovePersistedFilterDefinition> filter_definition_remover;
        static StreamId source_stream;
        static StreamId target_stream;
        static Exception exception;

        Establish context = () =>
        {
            filter_definition_remover = new Moq.Mock<ICanRemovePersistedFilterDefinition>();
            container.Setup(_ => _.Get(typeof(IFilterDefinitionRepositoryFor<TypeFilterWithEventSourcePartitionDefinition>))).Returns(filter_definition_remover.Object);
            source_stream = Guid.NewGuid();
            target_stream = Guid.NewGuid();
            var filter_processor = new TypeFilterWithEventSourcePartition(new TypeFilterWithEventSourcePartitionDefinition(source_stream, target_stream, Array.Empty<ArtifactId>(), false), Moq.Mock.Of<IWriteEventsToStreams>(), Moq.Mock.Of<ILogger>());
            filter_registry.Register(filter_processor).GetAwaiter().GetResult();
        };

        Because of = () => filter_registry.RemoveIfPersisted(target_stream).GetAwaiter().GetResult();

        It should_remove_persisted_filter_definition = () => filter_definition_remover.Verify(_ => _.RemovePersistedFilter(target_stream, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}
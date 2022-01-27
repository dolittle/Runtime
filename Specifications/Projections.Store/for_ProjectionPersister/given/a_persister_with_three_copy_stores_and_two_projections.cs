// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.Copies;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.given;

public class a_persister_with_three_copy_stores_and_two_projections : two_projections
{
    protected static Mock<IProjectionStore> projection_store;
    protected static Mock<IProjectionCopyStore> copy_store_one;
    protected static Mock<IProjectionCopyStore> copy_store_two;
    protected static Mock<IProjectionCopyStore> copy_store_three;

    protected static Mock<IMetricsCollector> metrics;

    protected static ProjectionPersister projection_persister;

    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        projection_store = new Mock<IProjectionStore>();
        projection_store
            .Setup(_ => _.TryReplace(
                It.IsAny<ProjectionId>(),
                It.IsAny<ScopeId>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<ProjectionState>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        projection_store
            .Setup(_ => _.TryRemove(
                It.IsAny<ProjectionId>(),
                It.IsAny<ScopeId>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        projection_store
            .Setup(_ => _.TryDrop(
                It.IsAny<ProjectionId>(),
                It.IsAny<ScopeId>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);

        copy_store_one = new Mock<IProjectionCopyStore>();
        copy_store_one
            .Setup(_ => _.ShouldPersistFor(
                It.IsAny<ProjectionDefinition>()
            )).Returns(true);
        copy_store_one
            .Setup(_ => _.TryReplace(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<ProjectionState>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        copy_store_one
            .Setup(_ => _.TryRemove(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        copy_store_one
            .Setup(_ => _.TryDrop(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        
        copy_store_two = new Mock<IProjectionCopyStore>();
        copy_store_two
            .Setup(_ => _.ShouldPersistFor(
                It.IsAny<ProjectionDefinition>()
            )).Returns(true);
        copy_store_two
            .Setup(_ => _.TryReplace(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<ProjectionState>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        copy_store_two
            .Setup(_ => _.TryRemove(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        copy_store_two
            .Setup(_ => _.TryDrop(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        
        copy_store_three = new Mock<IProjectionCopyStore>();
        copy_store_three
            .Setup(_ => _.ShouldPersistFor(
                It.IsAny<ProjectionDefinition>()
            )).Returns(true);
        copy_store_three
            .Setup(_ => _.TryReplace(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<ProjectionState>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        copy_store_three
            .Setup(_ => _.TryRemove(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<ProjectionKey>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);
        copy_store_three
            .Setup(_ => _.TryDrop(
                It.IsAny<ProjectionDefinition>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(true);

        metrics = new Mock<IMetricsCollector>();

        projection_persister = new ProjectionPersister(
            projection_store.Object,
            new[] {copy_store_one.Object, copy_store_two.Object, copy_store_three.Object},
            metrics.Object,
            NullLogger.Instance
        );
        
        cancellation_token = CancellationToken.None;
    };
}
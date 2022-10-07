// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Aggregates.for_AggregateRootInstances.given;

public class all_dependencies
{
    protected static AggregateRootId an_aggregate_root_id;
    protected static AggregateRoot an_aggregate_root;
    protected static AggregateRootInstance an_aggregate_root_instance;
    protected static AggregateRootInstances aggregate_root_instances;
    protected static IAggregateRoots aggregate_roots;
    protected static Mock<IFetchAggregateRootInstances> aggregates_fetcher;

    private Establish context = () =>
    {
        aggregate_roots = new AggregateRoots();
        aggregates_fetcher = new Mock<IFetchAggregateRootInstances>(MockBehavior.Strict);

        an_aggregate_root_id = new AggregateRootId("5276ed2c-7b00-48d7-a047-3adc1d2bb962", ArtifactGeneration.First);
        an_aggregate_root = new AggregateRoot(an_aggregate_root_id, "some alias");
        an_aggregate_root_instance = new AggregateRootInstance(an_aggregate_root_id, "an event source", AggregateRootVersion.Initial);
        aggregate_root_instances = new AggregateRootInstances(aggregate_roots, aggregates_fetcher.Object);
    };

    protected static void setup_aggregate_roots(params AggregateRoot[] roots)
    {
        foreach (var root in roots)
        {
            aggregate_roots.Register(root);
        }
    }

    protected static void setup_aggregate_root_instances_fetcher(params (AggregateRoot, IEnumerable<AggregateRootInstance>)[] rootsAndAggregates)
    {
        aggregates_fetcher
            .Setup(_ => _.FetchFor(Moq.It.IsAny<AggregateRootId>()))
            .Returns(Task.FromResult(Enumerable.Empty<AggregateRootInstance>()));
            
        foreach (var (root, aggregates ) in rootsAndAggregates)
        {
            aggregates_fetcher
                .Setup(_ => _.FetchFor(root.Identifier))
                .Returns(Task.FromResult(aggregates));
        }
    } 
}
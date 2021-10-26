// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.AggregateRoots;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Aggregates.for_Aggregates.given
{
    public class all_dependencies
    {
        protected static AggregateRoot an_aggregate_root;
        protected static Aggregate an_aggregate;
        protected static Aggregates aggregates;
        protected static Mock<IAggregateRoots> aggregate_roots;
        protected static Mock<IFetchAggregates> aggregates_fetcher;

        Establish context = () =>
        {
            aggregate_roots = new Mock<IAggregateRoots>(MockBehavior.Strict);
            aggregates_fetcher = new Mock<IFetchAggregates>(MockBehavior.Strict);

            an_aggregate_root = new AggregateRoot(new Artifact("5276ed2c-7b00-48d7-a047-3adc1d2bb962", ArtifactGeneration.First), "some alias");
            an_aggregate = new Aggregate("an event source", AggregateRootVersion.Initial);
            aggregates = new Aggregates(aggregate_roots.Object, aggregates_fetcher.Object);
        };

        protected static void setup_aggregate_roots(params AggregateRoot[] roots) => aggregate_roots.SetupGet(_ => _.All).Returns(roots);

        protected static void setup_aggregates_fetcher(params (AggregateRoot, IEnumerable<Aggregate>)[] rootsAndAggregates)
        {
            aggregates_fetcher.Setup(_ => _.FetchFor(Moq.It.IsAny<AggregateRoot>())).Returns(Task.FromResult(Enumerable.Empty<Aggregate>()));
            foreach (var (root, aggregates ) in rootsAndAggregates)
            {
                aggregates_fetcher.Setup(_ => _.FetchFor(root)).Returns(Task.FromResult(aggregates));
            }
        } 
    }
}

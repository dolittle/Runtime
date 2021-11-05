// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Aggregates.for_AggregateRoots.given
{
    public class all_dependencies
    {
        protected static AggregateRootId an_aggregate_root_id;
        protected static AggregateRoot an_aggregate_root;
        protected static AggregateRoots aggregate_roots;

        private Establish context = () =>
        {
            an_aggregate_root_id = new AggregateRootId("78c81c44-25ed-489e-a6db-bdc46fe3e026", ArtifactGeneration.First);
            an_aggregate_root = new AggregateRoot(an_aggregate_root_id, "some alias");
            aggregate_roots = new AggregateRoots();
        };
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Machine.Specifications;

namespace Dolittle.Runtime.Aggregates.for_AggregateRoots.given
{
    public class all_dependencies
    {
        protected static AggregateRoots aggregate_roots;
        protected static AggregateRoot AnAggregateRoot;
        
        Establish context = () =>
        {
            AnAggregateRoot = new AggregateRoot(new Artifact("78c81c44-25ed-489e-a6db-bdc46fe3e026", ArtifactGeneration.First), "some alias");
            aggregate_roots = new AggregateRoots();
        };
    }
}

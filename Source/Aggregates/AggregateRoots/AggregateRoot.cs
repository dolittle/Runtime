// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Aggregates.AggregateRoots
{
    public record AggregateRoot(Artifact Type, AggregateRootAlias Alias)
    {
        public AggregateRoot(Artifact type)
            : this(type, AggregateRootAlias.NotSet)
        { }
    }
}

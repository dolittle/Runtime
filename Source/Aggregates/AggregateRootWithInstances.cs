// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents an Aggregate Root with all of its instances.
    /// </summary>
    /// <param name="Identifier">The Aggregate Root identifier.</param>
    /// <param name="Instances">The instances of the Aggregate Root that has committed events.</param>
    public record AggregateRootWithInstances(AggregateRootId Identifier, IEnumerable<AggregateRootInstance> Instances);
}

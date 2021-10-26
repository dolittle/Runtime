// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.Aggregates.AggregateRoots
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateRoots"/>.
    /// </summary>
    [Singleton]
    public class AggregateRoots : IAggregateRoots
    {
        readonly ConcurrentDictionary<Artifact, AggregateRoot> _aggregateRoots = new();

        /// <inheritdoc />
        public IEnumerable<AggregateRoot> All => _aggregateRoots.Values;

        /// <inheritdoc />
        public void Register(AggregateRoot aggregateRoot)
            => _aggregateRoots.AddOrUpdate(aggregateRoot.Type, aggregateRoot, (_, _) => aggregateRoot);
    }
}

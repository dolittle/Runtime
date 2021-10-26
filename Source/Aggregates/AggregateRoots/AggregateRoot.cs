// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Aggregates.AggregateRoots
{
    /// <summary>
    /// Represents an Aggregate Root.
    /// </summary>
    /// <param name="Type">The Aggregate Root type.</param>
    /// <param name="Alias">The alias of the Aggregate Root.</param>
    public record AggregateRoot(Artifact Type, AggregateRootAlias Alias)
    {
        /// <summary>
        /// Initializes an new instance of the <see cref="AggregateRoot"/> record.
        /// </summary>
        /// <param name="type">The Aggregate Root type.</param>
        public AggregateRoot(Artifact type)
            : this(type, AggregateRootAlias.NotSet)
        { }
    }
}

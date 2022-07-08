// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates;

/// <summary>
/// Exception that gets thrown when there is no registered Aggregate Root with the given Aggregate Root identifier.
/// </summary>
public class NoAggregateRootWithId : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoAggregateRootWithId"/> class.
    /// </summary>
    /// <param name="id">The Aggregate Root Id.</param>
    public NoAggregateRootWithId(ArtifactId id)
        : base($"There is no registered Aggregate Root with Id '{id.Value}'")
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoAggregateRootWithId"/> class.
    /// </summary>
    /// <param name="alias">The Aggregate Root alias.</param>
    public NoAggregateRootWithId(AggregateRootAlias alias)
        : base($"There is no registered Aggregate Root with Alias '{alias.Value}'")
    { }
}
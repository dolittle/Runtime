// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Aggregates.Management;

/// <summary>
/// The exception that gets thrown when an Aggregate Root is not registered.
/// </summary>
public class AggregateRootIsNotRegistered : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootIsNotRegistered"/> class.
    /// </summary>
    /// <param name="aggregateRootId"></param>
    public AggregateRootIsNotRegistered(ArtifactId aggregateRootId)
        : base($"Aggregate Root with Id {aggregateRootId.Value} is not registered")
    { }
}
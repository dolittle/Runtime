// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Aggregates;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Exception that gets thrown when constructing a <see cref="CommittedAggregateEvents"/> with a higher expected version than the given events produce.
/// </summary>
public class MissingEventsForExpectedAggregateRootVersion : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingEventsForExpectedAggregateRootVersion"/> class.
    /// </summary>
    /// <param name="expectedVersion">The expected <see cref="AggregateRootVersion"/>.</param>
    /// <param name="actualVersion">The actual <see cref="AggregateRootVersion"/> produced by the given events.</param>
    public MissingEventsForExpectedAggregateRootVersion(AggregateRootVersion expectedVersion, AggregateRootVersion actualVersion)
        : base($"Events are missing for aggregate root. Expected version '{expectedVersion}', but provided events resulted in '{actualVersion}'.")
    {
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.List
{
    /// <summary>
    /// Represents the detailed information for an Aggregate Root.
    /// </summary>
    /// <param name="AggregateRootAlias">The Aggregate Root alias.</param>
    /// <param name="AggregateRootId">The Aggregate Root Id.</param>
    /// <param name="Instances">The number of Aggregate Root Instances.</param>
    public record AggregateRootDetailedView(string AggregateRootAlias, Guid AggregateRootId, ulong Instances);
}

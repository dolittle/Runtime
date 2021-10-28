// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.List
{
    /// <summary>
    /// Represents the simple information for an Aggregate Root.
    /// </summary>
    /// <param name="AggregateRoot">The Aggregate Root identifier.</param>
    /// <param name="Instances">The number of Aggregate Root Instances.</param>
    public record AggregateRootSimpleView(string AggregateRoot, ulong Instances);
}

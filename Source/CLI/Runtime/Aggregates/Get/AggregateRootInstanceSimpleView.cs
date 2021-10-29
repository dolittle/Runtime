// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.Get
{
    /// <summary>
    /// Represents the simple information for an Aggregate Root Instance.
    /// </summary>
    /// <param name="EventSource">The Event Source Id.</param>
    public record AggregateRootInstanceSimpleView(string EventSource);
}

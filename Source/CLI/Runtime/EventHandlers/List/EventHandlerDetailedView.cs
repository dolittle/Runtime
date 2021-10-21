// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.List
{
    /// <summary>
    /// Represents the detailed information for an Event Handler.
    /// </summary>
    /// <param name="Alias">The Event Handler alias.</param>
    /// <param name="EventHandler">The Event Handler identifier.</param>
    /// <param name="Scope">The Event Handler Scope.</param>
    /// <param name="Partitioned">Whether the Event Handler is partitioned.</param>
    /// <param name="Status">The status of the Event Handler.</param>
    /// <param name="LastSuccessfullyProcessed">The last time the Event Handler successfully processed an Event.</param>
    public record EventHandlerDetailedView(
        string Alias,
        Guid EventHandler,
        string Scope,
        string Partitioned,
        string Status,
        DateTimeOffset LastSuccessfullyProcessed);
}

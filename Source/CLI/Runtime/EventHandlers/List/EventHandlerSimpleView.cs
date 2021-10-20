// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.List
{
    /// <summary>
    /// Represents the base information for an Event Handler.
    /// </summary>
    /// <param name="EventHandler">The Event Handler identifier.</param>
    /// <param name="Scope">The Event Handler Scope.</param>
    /// <param name="Partitioned">Whether the Event Handler is partitioned.</param>
    /// <param name="Status">The status of the Event Handler.</param>
    public record EventHandlerSimpleView(string EventHandler, string Scope, string Partitioned, string Status);


}
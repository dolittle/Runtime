// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Represents the status of an Event Handler.
/// </summary>
/// <param name="Id">The identifier of the Event Handler.</param>
/// <param name="EventTypes">The event types that the Event Handler handles.</param>
/// <param name="Partitioned">Whether the Event Handler is partitioned or unpartitioned.</param>
/// <param name="Alias">The alias of the Event Handler.</param>
/// <param name="States">The states of the Event Handler for each Tenant.</param>
public record EventHandlerStatus(
    EventHandlerId Id,
    IEnumerable<Artifact> EventTypes,
    bool Partitioned,
    EventHandlerAlias Alias,
    IEnumerable<TenantScopedStreamProcessorStatus> States)
{
    /// <summary>
    /// Gets a value indicating whether the Event Handler has an alias.
    /// </summary>
    public bool HasAlias => !Alias.Equals(EventHandlerAlias.NotSet);
        
    /// <summary>
    /// Gets a value indicating whether the Event Handler is in the default Scope.
    /// </summary>
    public bool IsInDefaultScope => Id.Scope.Equals(ScopeId.Default);
}

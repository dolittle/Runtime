// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the information for an <see cref="EventHandler"/>.
    /// </summary>
    /// <param name="Id">The <see cref="EventHandlerId"/>.</param>
    /// <param name="Alias">The name alias of the Event Handler.</param>
    /// <param name="EventTypes">The event types.</param>
    /// <param name="Partitioned">Whether the Event Handler is partitioned.</param>
    public record EventHandlerInfo(EventHandlerId Id, EventHandlerAlias Alias, IEnumerable<ArtifactId> EventTypes, bool Partitioned)
    {
        public bool HasAlias => !string.IsNullOrEmpty(Alias.Value);
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Defines a system that resolves an Event Handler from <see cref="EventHandlerIdOrAlias"/>.
/// </summary>
public interface IResolveEventHandlerId
{
    /// <summary>
    /// Resolves the <see cref="EventHandlerId"/> from <see cref="EventHandlerIdOrAlias"/>.
    /// </summary>
    /// <param name="runtime">The address to the Runtime.</param>
    /// <param name="idOrAlias">The Event Handler Id or Alias.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the resolved <see cref="EventHandlerId"/>.</returns>
    Task<EventHandlerId> ResolveId(MicroserviceAddress runtime, EventHandlerIdOrAlias idOrAlias);
        
}
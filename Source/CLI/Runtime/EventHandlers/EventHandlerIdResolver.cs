// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Represents an implementation of the <see cref="IResolveEventHandlerId"/>.  
/// </summary>
public class EventHandlerIdResolver : IResolveEventHandlerId
{
    readonly IManagementClient _managementClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerIdResolver"/> class.
    /// </summary>
    /// <param name="managementClient">The <see cref="IManagementClient"/>.</param>
    public EventHandlerIdResolver(IManagementClient managementClient)
    {
        _managementClient = managementClient;
    }

    /// <inheritdoc />
    public async Task<EventHandlerId> ResolveId(MicroserviceAddress runtime, EventHandlerIdOrAlias idOrAlias)
    {
        if (!idOrAlias.IsAlias)
        {
            return idOrAlias.Id;
        }
        var statuses = await _managementClient.GetAll(runtime).ConfigureAwait(false);
        var status = statuses.FirstOrDefault(_ => WithAlias(_, idOrAlias));
            
        if (status == default)
        {
            throw new NoEventHandlerWithId(idOrAlias.Alias);
        }
        return status.Id;
    }

    static bool WithAlias(EventHandlerStatus status, EventHandlerIdOrAlias idOrAlias)
        => status.HasAlias && status.Alias.Equals(idOrAlias.Alias);
}
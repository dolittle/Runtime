// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Events.Contracts.EventTypes;

namespace Dolittle.Runtime.Events;

/// <summary>
/// Represents the implementation of <see cref="EventTypesBase"/>.
/// </summary>
public class EventTypesService : EventTypesBase
{
    readonly IEventTypes _eventTypes;
    /// <summary>
    /// Initializes a new instance of the <see cref="EventTypesService"/> class.
    /// </summary>
    /// <param name="eventTypes">The Event Types.</param>
    public EventTypesService(IEventTypes eventTypes)
    {
        _eventTypes = eventTypes;
    }

    /// <inheritdoc/>
    public override Task<EventTypeRegistrationResponse> Register(EventTypeRegistrationRequest request, ServerCallContext context)
    {
        _eventTypes.Register(request.HasAlias
            ? new EventType(request.EventType.ToArtifact(), request.Alias)
            : new EventType(request.EventType.ToArtifact()));

        return Task.FromResult(new EventTypeRegistrationResponse());
    }
}
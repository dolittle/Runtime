// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents the information for an <see cref="EventHandler"/>.
/// </summary>
/// <param name="Id">The identifier of the Event Handler.</param>
/// <param name="HasAlias">Whether or not an alias was provided by the Client for the Event Handler.</param>
/// <param name="Alias">The alias of the Event Handler.</param>
/// <param name="EventTypes">The Event types that the Event Handler handles..</param>
/// <param name="Partitioned">Whether the Event Handler is partitioned.</param>
/// <param name="Concurrency">How many events the handler can process simultaneously.</param>
public record EventHandlerInfo(EventHandlerId Id, bool HasAlias, EventHandlerAlias Alias, IEnumerable<ArtifactId> EventTypes, bool Partitioned, int Concurrency, StartFrom StartFrom, DateTimeOffset? StopAt);

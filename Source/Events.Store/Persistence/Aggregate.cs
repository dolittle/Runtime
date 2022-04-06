// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Persistence;

/// <summary>
/// Represents an aggregate as the unique combination of an aggregate root type <see cref="ArtifactId"/> and an <see cref="EventSourceId"/>.
/// </summary>
/// <param name="AggregateRoot">The aggregate root type id.</param>
/// <param name="EventSourceId">The event source id.</param>
public record Aggregate(ArtifactId AggregateRoot, EventSourceId EventSourceId);

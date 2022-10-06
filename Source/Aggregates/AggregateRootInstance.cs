// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Aggregates;

/// <summary>
/// Represents an Aggregate Root Instance.
/// </summary>
/// <remarks>
/// An instance of an Aggregate Root is an Event Source that has applied events.
/// </remarks>
/// <param name="Identifier">The identifier of the Aggregate Root.</param>
/// <param name="EventSource">The Event Source Id.</param>
/// <param name="Version">The Aggregate Root Version.</param>
public record AggregateRootInstance(AggregateRootId Identifier, EventSourceId EventSource, AggregateRootVersion Version);

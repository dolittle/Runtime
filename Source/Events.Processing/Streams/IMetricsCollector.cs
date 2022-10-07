// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Defines a system for collecting metrics about EventHandlers.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Increments to total number of stream processor registration attempts per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementRegistrations(EventProcessorKind kind);

    /// <summary>
    /// Increments to total number of failed stream processor registrations per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementFailedRegistrations(EventProcessorKind kind);

    /// <summary>
    /// Increments the total number of stream processor initialization attempts per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementInitializations(EventProcessorKind kind);

    /// <summary>
    /// Increments the total number of stream processor start attempts per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementStarts(EventProcessorKind kind);

    /// <summary>
    /// Increments the total number of failed stream processors per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementFailures(EventProcessorKind kind);

    /// <summary>
    /// Increments the total number of positions set per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementPositionSet(EventProcessorKind kind);
    
    /// <summary>
    /// Increments the total number of inital positions set for all tenants set per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementInitialPositionSetForAllTenants(EventProcessorKind kind);

    /// <summary>
    /// Increments the total number of event processing attempts by their processing time per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    /// <param name="elapsed">The time it took to process the event.</param>
    void IncrementEventsProcessed(EventProcessorKind kind, TimeSpan elapsed);

    /// <summary>
    /// Increments the total number of failed event processing attempts per event processor kind.
    /// </summary>
    /// <param name="kind">The event processors kind.</param>
    void IncrementFailedEventsProcessed(EventProcessorKind kind);
}

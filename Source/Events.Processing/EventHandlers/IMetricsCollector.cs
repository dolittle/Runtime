// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Defines a system for collecting metrics about EventHandlers.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Increments to total number of EventHandler registration attempts.
    /// </summary>
    /// <param name="info">The EventHandler information.</param>
    void IncrementRegistrationsTotal(EventHandlerInfo info);

    /// <summary>
    /// Increments to total number of EventHandler registration attempts that failed.
    /// </summary>
    /// <param name="info">The EventHandler information.</param>
    void IncrementFailedRegistrationsTotal(EventHandlerInfo info);
    
    /// <summary>
    /// Increments the total number of Events processed by an EventHandler.
    /// </summary>
    /// <param name="info">The EventHandler information.</param>
    /// <param name="tenant">The Tenant that the Event was processed for.</param>
    /// <param name="event">The Event that was processed.</param>
    /// <param name="processingTime">The time it took to process the Event by the EventProcessor.</param>
    void IncrementEventsProcessedTotal(EventHandlerInfo info, TenantId tenant, StreamEvent @event, TimeSpan processingTime);
    
    /// <summary>
    /// Increments the total number of Events that failed to be processed by an EventHandler.
    /// </summary>
    /// <param name="info">The EventHandler information.</param>
    /// <param name="tenant">The Tenant that the Event was processed for.</param>
    /// <param name="event">The Event that could not be processed.</param>
    void IncrementEventProcessingFailuresTotal(EventHandlerInfo info, TenantId tenant, StreamEvent @event);
}

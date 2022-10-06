// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Defines a system for collecting metrics about EventHandlers.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryGet"/>.
    /// </summary>
    void IncrementRegistrationsTotal();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryGet"/>.
    /// </summary>
    void IncrementFailedRegistrationsTotal();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryGet"/>.
    /// </summary>
    void IncrementEventsProcessedTotal();
}

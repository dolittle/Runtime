// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Defines a system for collecting metrics about EventHandlers.
/// </summary>
public interface IMetricsCollector
{
    void IncrementRegistrationsTotal(EventHandlerInfo info);

    void IncrementFailedRegistrationsTotal();

    void IncrementEventsProcessedTotal();
}

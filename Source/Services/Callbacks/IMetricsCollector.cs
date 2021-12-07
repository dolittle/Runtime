// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace Dolittle.Runtime.Services.Callbacks;

/// <summary>
/// Defines a system for collecting metrics about scheduled callbacks.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Increments the total number of callbacks registered.
    /// </summary>
    void IncrementTotalCallbacksRegistered();

    /// <summary>
    /// Increments the total number of callbacks invoked.
    /// </summary>
    void IncrementTotalCallbacksCalled();

    /// <summary>
    /// Adds to the total time spent invoking scheduled callbacks.
    /// </summary>
    /// <param name="elapsed">The time spent calling a callback.</param>
    void AddToTotalCallbackTime(TimeSpan elapsed);

    /// <summary>
    /// Increments the total number of callback invocations that failed.
    /// </summary>
    void IncrementTotalCallbacksFailed();

    /// <summary>
    /// Increments the total number of callbacks unregistered.
    /// </summary>
    void IncrementTotalCallbacksUnregistered();

    /// <summary>
    /// Increments the total number of times the callback schedule was missed.
    /// </summary>
    void IncrementTotalSchedulesMissed();

    /// <summary>
    /// Adds how much the scheduled was missed by to the total missed schedule time.
    /// </summary>
    /// <param name="elapsed">How much the schedule was missed by.</param>
    void AddToTotalSchedulesMissedTime(TimeSpan elapsed);

    /// <summary>
    /// Increments the total number of loops that failed invoking callbacks.
    /// </summary>
    void IncrementTotalCallbackLoopsFailed();
}
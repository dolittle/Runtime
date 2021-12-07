// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Defines a system for collecting metrics about event horizon consumer.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Increments the total number of event horizon events processed.
    /// </summary>
    void IncrementTotalEventHorizonEventsProcessed();

    /// <summary>
    /// Increments the total number of event horizon event writes failed.
    /// </summary>
    void IncrementTotalEventHorizonEventWritesFailed();

    /// <summary>
    /// Increments the total number of events fetched from the event fetcher.
    /// </summary>
    void IncrementTotalEventsFetched();

    /// <summary>
    /// Increment the total number of stream processors that has been started.
    /// </summary>
    void IncrementTotalStreamProcessorStarted();

    /// <summary>
    /// Increment the total number of stream processors that has been attemptedw started.
    /// </summary>
    void IncrementTotalStreamProcessorStartAttempts();
}
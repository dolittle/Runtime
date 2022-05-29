// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents the implementation of <see cref="IEventProcessorPolicies"/>.
/// </summary>
public class EventProcessorPolicies : IEventProcessorPolicies
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessorPolicies"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public EventProcessorPolicies(ILogger logger)
    {
        WriteEvent = Policy<EventLogSequenceNumber>
            .Handle<EventStoreUnavailable>(ex =>
                {
                    Log.EventStoreIsUnavailable(logger, ex);
                    return true;
                })
            .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 10)));
    }

    /// <inheritdoc />
    public IAsyncPolicy<EventLogSequenceNumber> WriteEvent { get; }
}

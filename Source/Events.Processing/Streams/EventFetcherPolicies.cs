// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Polly;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="IEventFetcherPolicies"/>.
/// </summary>
[Singleton]
public class EventFetcherPolicies : IEventFetcherPolicies
{
    /// <summary>
    /// Initialises a new 
    /// </summary>
    /// <param name="logger"></param>
    public EventFetcherPolicies(ILogger logger)
    {
        Fetching = Policy<Try<IEnumerable<StreamEvent>>>
            .Handle<EventStoreUnavailable>(exception =>
            {
                Log.EventStoreUnavailable(logger, exception);
                return true;
            })
            // TODO: Now we might be able to check for failures on try as well?
            .WaitAndRetryForeverAsync(attempt =>
                TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 10)));
    }

    /// <inheritdoc />
    public IAsyncPolicy<Try<IEnumerable<StreamEvent>>> Fetching { get; }
}

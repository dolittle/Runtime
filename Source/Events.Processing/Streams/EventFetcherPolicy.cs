// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;
using Polly;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines an <see cref="AsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.
    /// </summary>
    public class EventFetcherPolicy : IDefineAsyncPolicyForType
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFetcherPolicy"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventFetcherPolicy(ILogger<ICanFetchEventsFromStream> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Type Type => typeof(ICanFetchEventsFromStream);

        /// <inheritdoc/>
        public Polly.IAsyncPolicy Define() =>
            Polly.Policy
                .Handle<EventStoreUnavailable>(
                    _ =>
                    {
                        _logger.LogWarning(_, "Event Store is unavailable");
                        return true;
                    })
                .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 10)));
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System;
//using Dolittle.Runtime.Events.Store;
//using Microsoft.Extensions.Logging;
//using Dolittle.Runtime.Resilience;
//using Polly;
//
//namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;
//
///// <summary>
///// Defines the policy for processing an event from an event horizon.
///// </summary>
//public class EventProcessorPolicy : IDefineAsyncPolicyForType
//{
//    readonly ILogger<EventProcessor> _logger;
//
//    /// <summary>
//    /// Initializes a new instance of the <see cref="EventProcessorPolicy"/> class.
//    /// </summary>
//    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
//    public EventProcessorPolicy(ILogger<EventProcessor> logger) => _logger = logger;
//
//    /// <inheritdoc/>
//    public Type Type => typeof(EventProcessor);
//
//    /// <inheritdoc/>
//    public Polly.IAsyncPolicy Define()
//        => Polly.Policy
//            .Handle<EventStoreUnavailable>(
//                ex =>
//                {
//                    Log.EventStoreIsUnavailable(_logger, ex);
//                    return true;
//                })
//            .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 10)));
//}

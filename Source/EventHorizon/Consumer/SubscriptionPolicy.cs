// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Resilience;
using Microsoft.Extensions.Logging;
using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Defines an <see cref="AsyncPolicyFor{T}"/> <see cref="Subscription"/>.
/// </summary>
public class SubscriptionPolicy : IDefineAsyncPolicyForType
{
    readonly ILogger<SubscriptionPolicy> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionPolicy"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    public SubscriptionPolicy(ILogger<SubscriptionPolicy> logger)
        => _logger = logger;

    /// <inheritdoc/>
    public Type Type => typeof(Subscription);

    /// <inheritdoc/>
    public Polly.IAsyncPolicy Define()
        => Polly.Policy
            .Handle<Exception>(exception =>
            {
                _logger.SubscriptionConnectionFailed(exception);
                return true;
            })
            .WaitAndRetryForeverAsync(retryCount
                => TimeSpan.FromSeconds(
                    Math.Min(
                        Math.Pow(2, retryCount),
                        60)));
}
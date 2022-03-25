// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Microsoft.Extensions.Logging;
using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents the implementation of <see cref="ISubscriptionPolicies"/>.
/// </summary>
[Singleton]
public class SubscriptionPolicies : ISubscriptionPolicies
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionPolicies"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public SubscriptionPolicies(ILogger logger)
    {
        Connecting = Policy
            .Handle<Exception>(exception =>
            {
                logger.SubscriptionConnectionFailed(exception);
                return true;
            })
            .WaitAndRetryForeverAsync(retryCount => TimeSpan.FromSeconds(
                Math.Min(
                    Math.Pow(2, retryCount),
                    60)));
    }
    
    /// <inheritdoc />
    public IAsyncPolicy Connecting { get; }
}

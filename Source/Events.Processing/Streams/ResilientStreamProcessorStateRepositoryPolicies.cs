// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Polly;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="IResilientStreamProcessorStateRepositoryPolicies"/>.
/// </summary>
[Singleton]
public class ResilientStreamProcessorStateRepositoryPolicies : IResilientStreamProcessorStateRepositoryPolicies
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResilientStreamProcessorStateRepositoryPolicies"/> class.
    /// </summary>
    /// <param name="logger">The logger to use while retrying.</param>
    public ResilientStreamProcessorStateRepositoryPolicies(ILogger logger)
    {
        Persisting = Policy
            .Handle<Exception>(exception =>
            {
                Log.RetryPersistStreamProcessorState(logger, exception);
                return true;
            })
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1));

        Getting = Policy<Try<IStreamProcessorState>>
            .Handle<Exception>(exception =>
            {
                // TODO: Different log message for this
                Log.RetryPersistStreamProcessorState(logger, exception);
                return true;
            })
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1));
    }

    public IAsyncPolicy Persisting { get; }
    public IAsyncPolicy<Try<IStreamProcessorState>> Getting { get; }
}

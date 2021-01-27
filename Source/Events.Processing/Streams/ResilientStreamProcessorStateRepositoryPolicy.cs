// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;
using Polly;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines an <see cref="AsyncPolicyFor{T}" /> <see cref="ResilientStreamProcessorStateRepository" />.
    /// </summary>
    public class ResilientStreamProcessorStateRepositoryPolicy : IDefineAsyncPolicyForType
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResilientStreamProcessorStateRepositoryPolicy"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ResilientStreamProcessorStateRepositoryPolicy(ILogger<ResilientStreamProcessorStateRepository> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Type Type => typeof(ResilientStreamProcessorStateRepository);

        /// <inheritdoc/>
        public Polly.IAsyncPolicy Define() =>
            Polly.Policy
                .Handle<Exception>(
                    _ =>
                    {
                        _logger.Error(_, "Could not persist stream processor state to the event store, will retry in one second.");
                        return true;
                    })
                .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1));
    }
}

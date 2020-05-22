// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
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
                        _logger.Debug(_, "Event Store is unavailable");
                        return true;
                    })
                .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 10)));
    }
}

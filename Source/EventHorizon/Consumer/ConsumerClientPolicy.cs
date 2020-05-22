// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Resilience;
using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines an <see cref="AsyncPolicyFor{T}" /> <see cref="ConsumerClient" />.
    /// </summary>
    public class ConsumerClientPolicy : IDefineAsyncPolicyForType
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerClientPolicy"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ConsumerClientPolicy(ILogger<ConsumerClientPolicy> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Type Type => typeof(ConsumerClient);

        /// <inheritdoc/>
        public Polly.IAsyncPolicy Define() =>
            Polly.Policy
            .Handle<Exception>(_ =>
                {
                    _logger.Debug(_, "Unable to subscribe to event horizon");
                    return true;
                })
                .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 10)));
    }
}
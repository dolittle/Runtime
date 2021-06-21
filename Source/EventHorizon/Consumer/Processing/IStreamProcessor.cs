// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Defines a system for working with <see cref="ScopedStreamProcessor" /> registered for an Event Horizon Subscription.
    /// </summary>
    public interface IStreamProcessor
    {
        /// <summary>
        /// Start and wait for stream processor to finish.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartAndWait(CancellationToken cancellationToken);
    }
}

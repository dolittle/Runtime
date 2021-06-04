// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Defines a system for working with <see cref="ScopedStreamProcessor" /> registered for an Event Horizon Subscription.
    /// </summary>
    public interface IStreamProcessor
    {
        /// <summary>
        /// Try to start and wait for stream processor to finish.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns></returns>
        Task<Try<bool>> TryStartAndWait(CancellationToken cancellationToken);
    }
}

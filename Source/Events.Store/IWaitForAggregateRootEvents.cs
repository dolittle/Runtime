// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Defines a system that can wait for events to be applied to an Aggregate Root.
    /// </summary>
    public interface IWaitForAggregateRootEvents
    {
        /// <summary>
        /// Waits for an event to be applied to an Aggregate Root with a default timeout of 1 min.
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <param name="timeout">The <see cref="TimeSpan" /> for waiting.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
        Task WaitForEvent(ArtifactId aggregateRoot, TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Waits for an event to be applied to an Aggregate Root with a default timeout of 1 min.
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
        Task WaitForEvent(ArtifactId aggregateRoot, CancellationToken cancellationToken);
    }
}
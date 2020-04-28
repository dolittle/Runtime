// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a system that can register a <see cref="AbstractStreamProcessor" /> for all tenants.
    /// </summary>
    public interface IRegisterStreamProcessorForAllTenants
    {
        /// <summary>
        /// Register a <see cref="AbstractStreamProcessor" /> with the given <see cref="IEventProcessor" /> on the given source <see cref="StreamId" />.
        /// </summary>
        /// <param name="createEventProcessor">A <see cref="Func{TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="IEventProcessor" />.</param>
        /// <param name="getStreamDefinition">A <see cref="Func{TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="StreamDefinition" />.</param>
        /// <param name="streamProcessorRegistrations">The <see cref="StreamProcessorRegistrations" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <typeparam name="TEventProcessor">The <see cref="IEventProcessor" /> type.</typeparam>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Register<TEventProcessor>(Func<Task<TEventProcessor>> createEventProcessor, Func<Task<StreamDefinition>> getStreamDefinition, StreamProcessorRegistrations streamProcessorRegistrations, CancellationToken cancellationToken)
            where TEventProcessor : IEventProcessor;
    }
}

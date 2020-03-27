// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that knows how to register and start filters.
    /// </summary>
    public interface IFilters
    {
        /// <summary>
        /// Registers and starts processing an filters.
        /// </summary>
        /// <typeparam name="TResponse">The response <see cref="IMessage" />.</typeparam>
        /// <typeparam name="TRequest">The request <see cref="IMessage" />.</typeparam>
        /// <typeparam name="TFilterDefinition">The <see cref="IFilterDefinition" />.</typeparam>
        ///
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="dispatcher">The call dispatcher.</param>
        /// <param name="createEventProcessor">The callback for creating the <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A task.</returns>
        Task RegisterAndStartProcessing<TResponse, TRequest, TFilterDefinition>(
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId sourceStream,
            IReverseCallDispatcher<TResponse, TRequest> dispatcher,
            Func<IFilterProcessor<TFilterDefinition>> createEventProcessor,
            CancellationToken cancellationToken)
            where TResponse : IMessage
            where TRequest : IMessage
            where TFilterDefinition : IFilterDefinition;
    }
}
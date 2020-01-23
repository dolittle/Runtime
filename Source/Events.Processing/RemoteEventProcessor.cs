// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessorNew" />that processes the handling of an event.
    /// </summary>
    public class RemoteEventProcessor : IEventProcessorNew
    {
        readonly TenantId _tenant;
        readonly IHandlerService _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerProcessor"/> class.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="handler">The <see cref="IHandlerService" />.</param>
        public HandlerProcessor(
            TenantId tenant,
            EventProcessorId id,
            IHandlerService handler)
        {
            Identifier = id;
            _tenant = tenant;
            _handler = handler;
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEventEnvelope @event)
        {
            var result = await _handler.Handle(@event, Identifier).ConfigureAwait(false);
            return result;
        }
    }
}
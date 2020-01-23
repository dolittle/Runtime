// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the handling of an event.
    /// </summary>
    public class RemoteEventProcessor : IEventProcessor
    {
        readonly TenantId _tenant;
        readonly IRemoteProcessorService _remoteHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEventProcessor"/> class.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="remoteProcessor">The <see cref="IRemoteProcessorService" />.</param>
        public RemoteEventProcessor(
            TenantId tenant,
            EventProcessorId id,
            IRemoteProcessorService remoteProcessor)
        {
            Identifier = id;
            _tenant = tenant;
            _remoteHandler = remoteProcessor;
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEventEnvelope @event)
        {
            var result = await _remoteHandler.Handle(@event, Identifier).ConfigureAwait(false);
            return result;
        }
    }
}
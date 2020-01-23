// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IHandlerHub" />.
    /// </summary>
    [Singleton]
    public class HandlerHub : IHandlerHub
    {
        readonly StreamProcessorDictionary _streamProcessors;
        readonly IHandlerService _handlerService;
        readonly IStreamProcessorHub _streamProcessorHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerHub"/> class.
        /// </summary>
        /// <param name="handlerService">The <see cref="IHandlerService" />.</param>
        /// <param name="streamProcessorHub">The <see cref="IStreamProcessorHub" />.</param>
        public HandlerHub(
            IExecutionContextManager manager,
            ITenants tenants,
            IHandlerService handlerService,
            IStreamProcessorHub streamProcessorHub)
        {
            _handlerService = handlerService;
            _streamProcessorHub = streamProcessorHub;
        }

        /// <inheritdoc />
        public void Register(EventHandlerId handlerId, StreamId sourceStreamId, TenantId tenantId)
        {
            var streamProcessor = _streamProcessorHub.Register(new HandlerProcessor(tenantId, handlerId.Value, _handlerService), sourceStreamId, tenantId);
            _streamProcessors.Add(streamProcessor.Key, streamProcessor);
            streamProcessor.Start();
        }
    }
}
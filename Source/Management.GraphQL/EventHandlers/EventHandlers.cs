// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Management.GraphQL.EventHandlers
{
    public class EventHandlers
    {
        readonly IEventHandlers _eventHandlers;
        readonly IExecutionContextManager _executionContextManager;
        readonly IContainer _container;
        readonly ILogger _logger;

        public EventHandlers(IEventHandlers eventHandlers, IExecutionContextManager executionContextManager, IContainer container, ILogger logger)
        {
            _eventHandlers = eventHandlers;
            _executionContextManager = executionContextManager;
            _container = container;
            _logger = logger;
        }

        public async Task<IEnumerable<EventHandler>> AllForTenant(Guid tenantId)
        {
            _executionContextManager.CurrentFor(Microservice.NotSet, tenantId);
            var eventStore = _container.Get<FactoryFor<IEventStore>>()();
            var tailEventLogSequenceNumber = await eventStore.GetTailEventLogSequenceNumber().ConfigureAwait(false);

            return _eventHandlers.All.Select(_ => new EventHandler
            {
                Id = _.EventProcessor,
                Scope = _.Scope,
                FilterPosition = (int)_.FilterStreamProcessor?.GetScopedStreamProcessorFor(tenantId).CurrentState.Position.Value,
                EventProcessorPosition = (int)_.EventProcessorStreamProcessor?.GetScopedStreamProcessorFor(tenantId).CurrentState.Position.Value,
                TailEventLogSequenceNumber = (int)tailEventLogSequenceNumber.Value
            });
        }
    }
}
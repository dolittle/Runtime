// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventsDistributer"></see>.
    /// </summary>
    public class EventDistributor : IEventsDistributer
    {
        readonly CancellationToken _cancellationToken;
        readonly IObservable<EventEnvelope> _allStream;
        readonly IEventProcessorStates _eventProcessorStates;
        readonly IEventProcessors _eventProcessors;
        readonly IDictionary<EventStreamId, EventProcessorState> _streamAndStateMap = new Dictionary<EventStreamId, EventProcessorState>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDistributor"/> class.
        /// </summary>
        /// <param name="eventProcessorStates">The <see cref="IEventProcessorStates">event processor states</see>.</param>
        /// <param name="eventProcessors">The event processors.</param>
        public EventDistributor(
            IEventProcessorStates eventProcessorStates,
            IEventProcessors eventProcessors
        )
        {
            _eventProcessorStates = eventProcessorStates;
            _eventProcessors = eventProcessors;
            PopulateStreamAndStateMap();
        }

        /// <inheritdoc/>
        public IDictionary<EventStreamId, EventProcessorState> EventProcessorStates => new Dictionary<EventStreamId, EventProcessorState>(_streamAndStateMap);

        /// <inheritdoc/>
        public async Task StartDistribution()
        {
            await Task.Run(async () => {
                while (!_cancellationToken.IsCancellationRequested) {
                var @event = await _allStream
                    .Where(_ => true)
                    .FirstAsync();

                    Process(@event);
                }
            }).ConfigureAwait(false);
        }

        void PopulateStreamAndStateMap()
        {
            var list = new List<EventStreamId>();
            list.ForEach(id => {
                var state = _eventProcessorStates.Get(id);
                if (state.IsNullState) state = new EventProcessorState { Offset = EventStreamOffset.Start, ProcessingState = ProcessingState.Ok };
                _streamAndStateMap.Add(id, state);
            });
        }

        void Process(EventEnvelope eventEnvelope)
        {
            var id = new EventStreamId { Value = System.Guid.NewGuid() };
            var currentState = _eventProcessorStates.Get(id);
            if (CanProcess(currentState.ProcessingState))
            {
                var state = _eventProcessors.Process(id, eventEnvelope);

                UpdateState(id, state);
            }
        }

        EventProcessorState UpdateState(EventStreamId eventStreamId, ProcessingState processingState) => _eventProcessorStates.Update(eventStreamId, processingState);

        bool CanProcess(ProcessingState processingState) => processingState != ProcessingState.Stop;

        
    }
}
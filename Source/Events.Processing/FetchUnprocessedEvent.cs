// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchUnprocessedStream" />.
    /// </summary>
    public class FetchUnprocessedStream : IFetchUnprocessedStream
    {
        readonly FactoryFor<IEventStore> _getEventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchUnprocessedStream"/> class.
        /// </summary>
        /// <param name="getEventStore"><see cref="FactoryFor{IEventStore}"/> for <see cref="IEventStore"/>.</param>
        public FetchUnprocessedStream(FactoryFor<IEventStore> getEventStore)
        {
            _getEventStore = getEventStore;
        }

        /// <inheritdoc />
        public IObservable<CommittedEventEnvelope> GetUnprocessedStream(StreamPosition streamPostion)
        {
            // using (var eventStore = _getEventStore())
            // {
            //     var eventStream = eventStore.FetchAllEventsOfTypeAfter(eventType, committedEventVersion.Major);
            // }
            return new List<CommittedEventEnvelope>().ToObservable();
        }
    }
}
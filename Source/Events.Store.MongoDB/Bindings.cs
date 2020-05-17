// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// Binds up the bindings related to the event store.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IEventStoreConnection>().To<EventStoreConnection>();
            builder.Bind<IStreamProcessorStateRepository>().To<StreamProcessorStateRepository>();
            builder.Bind<IStreamDefinitionRepository>().To<StreamDefinitionRepository>();
            builder.Bind<IEventFetchers>().To<EventFetchers>();
            builder.Bind<IWriteEventsToStreams>().To<EventsToStreamsWriter>();
            builder.Bind<IWriteEventsToStreamCollection>().To<EventsToStreamsWriter>();
            builder.Bind<IWriteEventHorizonEvents>().To<EventHorizonEventsWriter>();
            builder.Bind<IWriteEventsToPublicStreams>().To<EventsToPublicStreamsWriter>();
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Streams;

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
            builder.Bind<IFetchEventsFromStreams>().To<EventsFromStreamsFetcher>();
            builder.Bind<IWriteEventsToStreams>().To<EventsToStreamsWriter>();
            builder.Bind<IFetchEventTypesFromStreams>().To<EventTypesFromStreamsFetcher>();
            builder.Bind<IStreamProcessorStateRepository>().To<StreamProcessorStateRepository>();
            builder.Bind<IStreamsMetadata>().To<StreamsMetadata>();
            builder.Bind<IFilterDefinitionRepositoryFor<TypeFilterWithEventSourcePartitionDefinition>>().To<TypePartitionFilterDefinitionRepository>();
            builder.Bind<IWriteEventHorizonEvents>().To<EventHorizonEventsWriter>();
        }
    }
}
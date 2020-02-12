// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Processing;

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
            builder.Bind<IWriteEventsToStreams>().To<EventsToStreamsWriter>();
            builder.Bind<IFetchEventsFromStreams>().To<EventsFromStreamsFetcher>();
            builder.Bind<IStreamProcessorStateRepository>().To<StreamProcessorStateRepository>();
        }
    }
}
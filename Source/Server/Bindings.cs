// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents <see cref="ICanProvideBindings">bindings</see> for defaults for the server.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IWriteEventsToStreams>().To<NullEventToStreamsWriter>();
            builder.Bind<IStreamProcessorStateRepository>().To<NullStreamProcessorStateRepository>();
            builder.Bind<IFetchEventsFromStreams>().To<NullFetchEventsFromStreams>();
        }
    }
}
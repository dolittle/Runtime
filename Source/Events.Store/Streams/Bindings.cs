// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Binds up the bindings related to the event store.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IWaitForEventInStream>().To<StreamEventWatcher>();
            builder.Bind<IWaitForEventInPublicStream>().To<StreamEventWatcher>();
            builder.Bind<INotifyOfStreamEvents>().To<StreamEventWatcher>();
            builder.Bind<INotifyOfPublicStreamEvents>().To<StreamEventWatcher>();
        }
    }
}

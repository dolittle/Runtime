// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.MongoDB.State;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Projections.Store.MongoDB
{
    /// <summary>
    /// Binds up the bindings related to the event store.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IProjectionDefinitions>().To<ProjectionDefinitions>();
            builder.Bind<IProjectionStates>().To<ProjectionStates>();
        }
    }
}

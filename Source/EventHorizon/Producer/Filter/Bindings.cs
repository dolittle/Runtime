// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.EventHorizon.Producer.Filter
{
    /// <summary>
    /// Binds up the bindings related to the event store.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<ICanValidateFilterFor<PublicFilterDefinition>>().To<PublicFilterValidator>();
        }
    }
}
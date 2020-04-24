// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanProvideBindings "/>.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IPerformActionOnAllTenants>().To<ActionOnAllTenantsPerformer>();
            builder.Bind<IPerformAsynchronousActionOnAllTenants>().To<AsynchronousActionOnAllTenantsPerformer>();
        }
    }
}

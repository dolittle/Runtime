// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Provides bindings for Events relativity.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<ITenantOffsetRepository>().To<TenantOffsetRepository>();
        }
    }
}
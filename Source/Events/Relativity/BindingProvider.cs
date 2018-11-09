/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.DependencyInversion;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Provides bindings for Events relativity
    /// </summary>
    public class BindingProvider : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<ITenantOffsetRepository>().To<TenantOffsetRepository>();
        }
    }
}
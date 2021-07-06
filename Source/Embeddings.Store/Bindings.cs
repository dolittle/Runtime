// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Represents the bindings for embeddings store.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IConvertProjectionKeysToEventSourceIds>().To<ProjectionKeyToEventSourceIdConverter>();
        }
    }
}
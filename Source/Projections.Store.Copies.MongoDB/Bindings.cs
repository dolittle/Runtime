// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Represents a provide for bindings in <see cref="Dolittle.Runtime.Projections.Store.Copies.MongoDB"/>.
/// </summary>
public class Bindings : ICanProvideBindings
{
    /// <inheritdoc />
    public void Provide(IBindingProviderBuilder builder)
    {
        builder.Bind<IProjectionCopyStore>().To<MongoDBProjectionCopyStore>();
    }
}

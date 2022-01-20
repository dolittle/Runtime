// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents a <see cref="ICanProvideBindings">binding provider</see> for services.
/// </summary>
public class Bindings : ICanProvideBindings
{
    /// <inheritdoc/>
    public void Provide(IBindingProviderBuilder builder)
    {
        builder.Bind<IIdentifyRequests>().To<HeaderRequestIdentifier>();
    }
}
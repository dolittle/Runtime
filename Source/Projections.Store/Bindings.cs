// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Metrics;

namespace Dolittle.Runtime.Projections.Store;


/// <summary>
/// Represents bindings for <see cref="Dolittle.Runtime.Projections.Store"/>.
/// </summary>
public class Bindings : ICanProvideBindings
{
    /// <inheritdoc />
    public void Provide(IBindingProviderBuilder builder)
    {
        builder.Bind<ICanProvideMetrics>().To<MetricsCollector>();
    }
}

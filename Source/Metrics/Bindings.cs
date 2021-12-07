// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.DependencyInversion;
using Prometheus;

namespace Dolittle.Runtime.Metrics;

/// <summary>
/// Represents <see cref="ICanProvideBindings">bindings</see> for the metrics system.
/// </summary>
public class Bindings : ICanProvideBindings
{
    /// <inheritdoc/>
    public void Provide(IBindingProviderBuilder builder)
    {
        var collectorRegistry = new CollectorRegistry();
        var constructors = typeof(Prometheus.MetricFactory).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        var constructor = constructors[0];
        var prometheusFactory = constructor.Invoke(new object[] { collectorRegistry }) as Prometheus.MetricFactory;
        builder.Bind<Prometheus.MetricFactory>().To(prometheusFactory);
        builder.Bind<CollectorRegistry>().To(collectorRegistry);
    }
}
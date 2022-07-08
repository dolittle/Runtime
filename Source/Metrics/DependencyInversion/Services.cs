// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Metrics.DependencyInversion;

public class Services : ICanAddServicesForTypesWith<MetricsAttribute>
{
    /// <inheritdoc />
    public void AddServiceFor(Type type, MetricsAttribute attribute, IServiceCollection services)
    {
        services.AddSingleton(new MetricCollector(type));
        services.AddSingleton(type);
        // TODO: It would be nice if we could automatically force these interfaces to also to be singletons?
    }
}

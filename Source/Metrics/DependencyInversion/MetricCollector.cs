// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Metrics.DependencyInversion;

public class MetricCollector
{
    public Type CollectorType { get; }

    public MetricCollector(Type collectorType)
    {
        CollectorType = collectorType;
    }
}

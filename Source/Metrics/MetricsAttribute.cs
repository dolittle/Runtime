// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Metrics;

/// <summary>
/// Indicates that a class can provide metrics to be collected by the metrics system.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MetricsAttribute : Attribute
{
    // TODO: This attribute could have an argument to set prefixes for all metrics, so we can shorten some of the code in the other places in the runtime
}

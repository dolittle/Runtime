// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Metrics;

namespace Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;

public class RuntimeMetrics
{
    public const string SourceName = "Dolittle.Runtime";
    public static readonly Meter Meter = new(SourceName);
}

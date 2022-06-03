// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Server.Tracing;

[Configuration("opentelemetry")]
public class OpenTelemetryConfiguration
{
    /// <summary>
    /// OTLP reporting endpoint
    /// </summary>
    public string Endpoint { get; set; }
    public string ServiceName { get; set; }
    
    public bool Logging { get; set; } = true;
    public bool Tracing { get; set; } = true;
    public bool Metrics { get; set; } = true;
}

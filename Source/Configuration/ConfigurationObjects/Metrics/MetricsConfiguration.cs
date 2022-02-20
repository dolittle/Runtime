// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Metrics;

/// <summary>
/// Represents the configuration for Metrics.
/// </summary>
[Config("metrics")]
public record MetricsConfiguration(int Port);

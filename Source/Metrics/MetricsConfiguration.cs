// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents the <see cref="IConfigurationObject"/> for Metrics.
    /// </summary>
    [Name("metrics")]
    public record MetricsConfiguration(int Port) : IConfigurationObject;
}
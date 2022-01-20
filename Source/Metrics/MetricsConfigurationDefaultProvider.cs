// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Metrics;

/// <summary>
/// Represents the <see cref="ICanProvideDefaultConfigurationFor{T}">default provider</see> for <see cref="MetricsConfiguration"/>.
/// </summary>
public class MetricsConfigurationDefaultProvider : ICanProvideDefaultConfigurationFor<MetricsConfiguration>
{
    /// <inheritdoc/>
    public MetricsConfiguration Provide()
    {
        return new MetricsConfiguration(9700);
    }
}
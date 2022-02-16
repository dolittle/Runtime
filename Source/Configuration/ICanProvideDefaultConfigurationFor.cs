// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Defines a system that is capable of providing default instances of specific
/// <see cref="IConfigurationObject">configuration objects</see>.
/// </summary>
/// <typeparam name="TConfiguration">Type of <see cref="IConfigurationObject"/>.</typeparam>
public interface ICanProvideDefaultConfigurationFor<TConfiguration>
    where TConfiguration : IConfigurationObject
{
    /// <summary>
    /// Provide an instance of the <see cref="IConfigurationObject"/>.
    /// </summary>
    /// <returns><see cref="IConfigurationObject"/> instance.</returns>
    TConfiguration Provide();
}

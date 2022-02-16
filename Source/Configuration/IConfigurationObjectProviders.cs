// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents all the providers for <see cref="IConfigurationObject"/>.
/// </summary>
public interface IConfigurationObjectProviders
{
    /// <summary>
    /// Provide an instance of <see cref="IConfigurationObject"/>.
    /// </summary>
    /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject"/>.</param>
    /// <returns>An instance if provided - throws an exception if it can't provide it.</returns>
    /// <exception cref="MultipleProvidersProvidingConfigurationObject">Thrown when there are multiple providers providing <see cref="IConfigurationObject"/>.</exception>
    /// <exception cref="MissingProviderForConfigurationObject">Thrown when there are no providers capable of providing a given <see cref="IConfigurationObject"/>.</exception>
    object Provide(Type type);
}
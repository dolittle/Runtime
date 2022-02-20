// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system that can add services to a <see cref="IServiceCollection"/>.
/// </summary>
public interface ICanAddServices
{
    /// <summary>
    /// Adds services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services into.</param>
    void AddTo(IServiceCollection services);
}

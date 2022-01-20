// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Conventions;

/// <summary>
/// Defines a manager for binding conventions.
/// </summary>
public interface IBindingConventionManager
{
    /// <summary>
    /// Discover bindings and initialize.
    /// </summary>
    /// <returns>A <see cref="IBindingCollection"/>.</returns>
    IBindingCollection DiscoverAndSetupBindings();
}
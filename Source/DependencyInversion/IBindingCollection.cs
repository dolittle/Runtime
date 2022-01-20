// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a collection for holding bindings.
/// </summary>
public interface IBindingCollection : IEnumerable<Binding>
{
    /// <summary>
    /// Check if there is a binding for a specific type by generic parameter.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> to check if has binding.</typeparam>
    /// <returns>true if there is a binding, false if not.</returns>
    bool HasBindingFor<T>();

    /// <summary>
    /// Check if there is a binding for a specific type.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check if has binding.</param>
    /// <returns>true if there is a binding, false if not.</returns>
    bool HasBindingFor(Type type);
}
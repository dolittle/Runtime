// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Represents a set of discovered classes grouped by DI lifecycle.
/// </summary>
/// <param name="SingletonClasses">The discovered classes that should be registered as singleton.</param>
/// <param name="ScopedClasses">The discovered classes that should be registered as scoped.</param>
/// <param name="TransientClasses">The discovered classes that should be registered as transient.</param>
public record ClassesByLifecycle(
    Type[] SingletonClasses,
    Type[] ScopedClasses,
    Type[] TransientClasses);

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Lifecycle;

/// <summary>
/// Indicates that the class should be registered as a scoped instance in a DI container.
/// Meaning that a single instance will be shared between container scopes.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ScopedAttribute : Attribute
{
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Lifecycle;

/// <summary>
/// Indicates that the class should be registered as a singleton instance in a DI container.
/// Meaning that a single instance will be created per container.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SingletonAttribute : Attribute
{
}

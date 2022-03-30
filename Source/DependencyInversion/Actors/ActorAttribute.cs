// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Scoping;

/// <summary>
/// Indicates that the class should be registered as an actor in a DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ActorAttribute : Attribute
{
}

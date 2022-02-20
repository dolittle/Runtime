// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Indicates that the class should not be registered automatically in a DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DisableAutoRegistrationAttribute : Attribute
{
}

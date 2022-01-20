// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Scopes;

/// <summary>
/// Represents a <see cref="IScope"/> for singleton - one instance per process
/// Adhering to the highlander principle; there can be only one.
/// </summary>
public class Singleton : IScope
{
}
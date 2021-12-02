// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Scopes;

/// <summary>
/// Represents a <see cref="IScope"/> for transient - meaning that there will be a new instance
/// for every activation.
/// </summary>
public class Transient : IScope
{
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Represents a comparer for comparing assemblies, typically used in Distinct().
/// </summary>
public class AssemblyComparer : IEqualityComparer<Assembly>
{
    /// <inheritdoc/>
    public bool Equals(Assembly x, Assembly y)
    {
        return x.FullName == y.FullName;
    }

    /// <inheritdoc/>
    public int GetHashCode(Assembly obj)
    {
        return obj.GetHashCode();
    }
}
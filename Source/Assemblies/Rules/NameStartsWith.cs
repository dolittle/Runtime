// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Specifications;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies.Rules;

/// <summary>
/// Represents a <see cref="Specification{T}">rule</see> specific to <see cref="Library">libraries</see> to filter on specific name starting with.
/// </summary>
public class NameStartsWith : Specification<Library>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NameStartsWith"/> class.
    /// </summary>
    /// <param name="name">Name to check if <see cref="Library"/> starts with.</param>
    public NameStartsWith(string name) => Predicate = library => library.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase);
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Specifications;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies.Rules;

/// <summary>
/// Rule representing an exception for <see cref="IncludeAllRule"/>,
/// excluding assembies starting with.
/// </summary>
public class ExceptProjectLibraries : Specification<Library>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptProjectLibraries"/> class.
    /// </summary>
    public ExceptProjectLibraries() => Predicate = library => library.Type.Equals("project", StringComparison.InvariantCultureIgnoreCase);
}
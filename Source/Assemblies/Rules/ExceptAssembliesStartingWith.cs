// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Specifications;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies.Rules
{
    /// <summary>
    /// Rule representing an exception for <see cref="IncludeAllRule"/>,
    /// excluding assembies starting with.
    /// </summary>
    public class ExceptAssembliesStartingWith : Specification<Library>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptAssembliesStartingWith"/> class.
        /// </summary>
        /// <param name="names">Params of assembly names that starts with to exclude.</param>
        public ExceptAssembliesStartingWith(params string[] names)
        {
            Predicate = a => !names.Any(n => a.Name.StartsWith(n, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

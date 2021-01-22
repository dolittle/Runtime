// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Specifications;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies.Rules
{
    /// <summary>
    /// Represents a <see cref="Specification{T}">rule</see> specific to <see cref="Assembly">assemblies</see>.
    /// and used for the <see cref="Assemblies"/>.
    /// </summary>
    public class ExcludeAllRule : Specification<Library>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeAllRule"/> class.
        /// </summary>
        public ExcludeAllRule()
        {
            Predicate = a => false;
        }
    }
}

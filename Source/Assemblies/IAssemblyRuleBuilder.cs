// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Specifications;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Defines a rule builder for building configuration for assemblies and how to include
    /// or exclude assemblies.
    /// </summary>
    public interface IAssemblyRuleBuilder
    {
        /// <summary>
        /// Gets or sets the specification to use.
        /// </summary>
        Specification<Library> Specification { get; set; }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Specifications;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies.Configuration
{
    /// <summary>
    /// Represents the configuration for Assemblies.
    /// </summary>
    public class AssembliesConfiguration
    {
        readonly IAssemblyRuleBuilder _assemblyRuleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssembliesConfiguration"/> class.
        /// </summary>
        /// <param name="assemblyRuleBuilder"><see cref="IAssemblyRuleBuilder"/> that builds the rules.</param>
        public AssembliesConfiguration(IAssemblyRuleBuilder assemblyRuleBuilder)
        {
            _assemblyRuleBuilder = assemblyRuleBuilder;
        }

        /// <summary>
        /// Gets the specification used to specifying which assemblies to include.
        /// </summary>
        public Specification<Library> Specification => _assemblyRuleBuilder.Specification;
    }
}

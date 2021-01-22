// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Assemblies.Configuration;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Represents an implementation of <see cref="IAssemblyFilters"/>.
    /// </summary>
    public class AssemblyFilters : IAssemblyFilters
    {
        readonly AssembliesConfiguration _assembliesConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFilters"/> class.
        /// </summary>
        /// <param name="assembliesConfiguration">The <see cref="AssembliesConfiguration"/>.</param>
        public AssemblyFilters(AssembliesConfiguration assembliesConfiguration)
        {
            _assembliesConfiguration = assembliesConfiguration;
        }

        /// <inheritdoc/>
        public bool ShouldInclude(Library library)
        {
            return _assembliesConfiguration.Specification.IsSatisfiedBy(library);
        }
    }
}

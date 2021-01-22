// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Assemblies.Rules;

namespace Dolittle.Runtime.Assemblies.Configuration
{
    /// <summary>
    /// Represents a builder for building configuration used by <see cref="Assemblies"/>.
    /// </summary>
    public class AssembliesConfigurationBuilder
    {
        /// <summary>
        /// Gets the <see cref="IAssemblyRuleBuilder">rule builder</see> used.
        /// </summary>
        public IAssemblyRuleBuilder RuleBuilder { get; private set; }

        /// <summary>
        /// Exclude all assemblies with possible exceptions.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="ExcludeAll">configuration object</see> for the rule.
        /// </returns>
        public ExcludeAll ExcludeAll()
        {
            var excludeAll = new ExcludeAll();
            RuleBuilder = excludeAll;
            return excludeAll;
        }

        /// <summary>
        /// Include all assemblies with possible exceptions.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="IncludeAll">configuration object</see> for the rule.
        /// </returns>
        public IncludeAll IncludeAll()
        {
            var includeAll = new IncludeAll();
            RuleBuilder = includeAll;
            return includeAll;
        }
    }
}

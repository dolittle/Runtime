// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Specifications;

namespace Dolittle.Runtime.Assemblies.Rules;

/// <summary>
/// Provides extensions for <see cref="IAssemblyRuleBuilder"/>.
/// </summary>
public static class AssemblyRuleBuilderExtensions
{
    /// <summary>
    /// Excludes specified assemblies.
    /// </summary>
    /// <param name="assemblyBuilder"><see cref="IAssemblyRuleBuilder"/> to build upon.</param>
    /// <param name="names">Names that assemblies should not be starting with.</param>
    /// <returns>Chained <see cref="IAssemblyRuleBuilder"/>.</returns>
    public static IAssemblyRuleBuilder ExcludeAssembliesStartingWith(this IAssemblyRuleBuilder assemblyBuilder, params string[] names)
    {
        assemblyBuilder.Specification = assemblyBuilder.Specification.And(new ExceptAssembliesStartingWith(names));
        return assemblyBuilder;
    }
}
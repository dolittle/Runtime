// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Specifications;

namespace Dolittle.Runtime.Assemblies.Rules;

/// <summary>
/// Extensions for <see cref="ExcludeAll"/>.
/// </summary>
public static class ExcludeAllExtensions
{
    /// <summary>
    /// Include project libraries.
    /// </summary>
    /// <param name="excludeAll"><see cref="ExcludeAll">configuration object</see>.</param>
    /// <returns>Chain of <see cref="ExcludeAll">configuration object</see>.</returns>
    public static ExcludeAll ExceptProjectLibraries(this ExcludeAll excludeAll)
    {
        var specification = excludeAll.Specification;

        specification = specification.Or(new ExceptProjectLibraries());

        excludeAll.Specification = specification;
        return excludeAll;
    }

    /// <summary>
    /// Include Dolittle libraries.
    /// </summary>
    /// <param name="excludeAll"><see cref="ExcludeAll">configuration object</see>.</param>
    /// <returns>Chain of <see cref="ExcludeAll">configuration object</see>.</returns>
    public static ExcludeAll ExceptDolittleLibraries(this ExcludeAll excludeAll)
    {
        var specification = excludeAll.Specification;
        specification = specification.Or(new NameStartsWith("Dolittle"));
        excludeAll.Specification = specification;
        return excludeAll;
    }

    /// <summary>
    /// Include specific assemblies that start with a specific name.
    /// </summary>
    /// <param name="excludeAll"><see cref="ExcludeAll">configuration object</see>.</param>
    /// <param name="names">Params of names to include.</param>
    /// <returns>Chain of <see cref="ExcludeAll">configuration object</see>.</returns>
    public static ExcludeAll ExceptAssembliesStartingWith(this ExcludeAll excludeAll, params string[] names)
    {
        var specification = excludeAll.Specification;
        names.ForEach(_ => specification = specification.Or(new NameStartsWith(_)));
        excludeAll.Specification = specification;
        return excludeAll;
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Specifications;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies.Rules;

/// <summary>
/// Represents the <see cref="IAssemblyRuleBuilder">builder</see> for building the <see cref="IncludeAllRule"/> and
/// possible exceptions.
/// </summary>
public class ExcludeAll : IAssemblyRuleBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeAll"/> class.
    /// </summary>
    public ExcludeAll()
    {
        Specification = new ExcludeAllRule();
    }

    /// <summary>
    /// Gets or sets the <see cref="IncludeAllRule"/>.
    /// </summary>
    public Specification<Library> Specification { get; set; }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// The exception that gets thrown when multiple Projections are registered with the specified Identifier or Alias, in a Scope
/// </summary>
public class MultipleProjectionsWithIdentifierOrAliasInScope : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleProjectionsWithIdentifierOrAliasInScope"/> class.
    /// </summary>
    /// <param name="identifierOrAlias">The identifier or alias of the Projection.</param>
    /// <param name="scope">The scope of the Projection.</param>
    /// <param name="count">The number of Projections found.</param>
    public MultipleProjectionsWithIdentifierOrAliasInScope(string identifierOrAlias, ScopeId scope, int count)
        : base($"{count} projections with alias '{identifierOrAlias}' was found in scope {scope}. Please use the identifier to specify one")
    {
    }
}

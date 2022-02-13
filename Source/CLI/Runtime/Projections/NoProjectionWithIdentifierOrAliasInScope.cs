// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// The exception that gets thrown when no Projection is registered with the specified Identifier or Alias, in a Scope
/// </summary>
public class NoProjectionWithIdentifierOrAliasInScope : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoProjectionWithIdentifierOrAliasInScope"/> class.
    /// </summary>
    /// <param name="identifierOrAlias">The identifier or alias of the Projection.</param>
    /// <param name="scope">The scope of the Projection.</param>
    public NoProjectionWithIdentifierOrAliasInScope(string identifierOrAlias, ScopeId scope)
        : base($"No projection with identifier or alias '{identifierOrAlias}' was found in scope {scope}")
    {
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Defines a system that can resolve <see cref="ProjectionIdentifierAndScope"/>.
/// </summary>
public interface IResolveProjectionIdAndScope
{
    /// <summary>
    /// Resolves the <see cref="ProjectionIdentifierAndScope"/> from an identifier or alias, and an optional scope
    /// </summary>
    /// <param name="runtime">The address of the Runtime.</param>
    /// <param name="identifierOrAlias">The Projection identifier or alias.</param>
    /// <param name="scope">The optional scope of the Projection.</param>
    /// <returns>T <see cref="Task"/> that, when resolved, returns a <see cref="Try"/> with the resolved <see cref="ProjectionIdentifierAndScope"/>.</returns>
    Task<Try<ProjectionIdentifierAndScope>> Resolve(MicroserviceAddress runtime, string identifierOrAlias, ScopeId scope = null);
}

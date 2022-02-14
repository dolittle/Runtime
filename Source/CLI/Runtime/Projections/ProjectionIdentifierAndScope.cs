// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Represents the unique pair of <see cref="ProjectionId"/> and <see cref="ScopeId"/> that identifies a Projection.
/// </summary>
/// <param name="Identifier">The <see cref="ProjectionId"/> of the Projection.</param>
/// <param name="Scope">The <see cref="ScopeId"/> of the Projection.</param>
public record ProjectionIdentifierAndScope(ProjectionId Identifier, ScopeId Scope);

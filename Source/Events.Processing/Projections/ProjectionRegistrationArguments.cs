// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents the runtime representation of the projection registration arguments.
/// </summary>
/// <param name="ExecutionContext">The execution context.</param>
/// <param name="ProjectionDefinition">The projection definition.</param>
public record ProjectionRegistrationArguments(
    ExecutionContext ExecutionContext,
    ProjectionDefinition ProjectionDefinition);

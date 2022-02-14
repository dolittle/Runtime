// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents the runtime representation of the projection registration arguments.
/// </summary>
public record ProjectionRegistrationArguments
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionRegistrationArguments"/> class.
    /// </summary>
    /// <param name="executionContext">The execution context of the Client while registering.</param>
    /// <param name="projectionDefinition">The Projection definition.</param>
    public ProjectionRegistrationArguments(ExecutionContext executionContext, ProjectionDefinition projectionDefinition)
    {
        ExecutionContext = executionContext;
        ProjectionDefinition = projectionDefinition;
        Alias = ProjectionAlias.NotSet;
        HasAlias = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionRegistrationArguments"/> class.
    /// </summary>
    /// <param name="executionContext">The execution context of the Client while registering.</param>
    /// <param name="projectionDefinition">The Projection definition.</param>
    /// <param name="alias">The alias of the Projection.</param>
    public ProjectionRegistrationArguments(ExecutionContext executionContext, ProjectionDefinition projectionDefinition, ProjectionAlias @alias)
    {
        ExecutionContext = executionContext;
        ProjectionDefinition = projectionDefinition;
        Alias = alias;
        HasAlias = true;
    }

    /// <summary>
    /// Gets the execution context of the Client while registering.
    /// </summary>
    public ExecutionContext ExecutionContext { get; }
    
    /// <summary>
    /// Gets the Projection definition.
    /// </summary>
    public ProjectionDefinition ProjectionDefinition { get; }
    
    /// <summary>
    /// Gets the alias of the Projection if set, or <see cref="ProjectionAlias.NotSet"/> if not passed from the Client.
    /// </summary>
    public ProjectionAlias Alias { get; }
    
    /// <summary>
    /// Gets a value indicating whether or not the Client passed along an alias for the Projection.
    /// </summary>
    public bool HasAlias { get;  }
}

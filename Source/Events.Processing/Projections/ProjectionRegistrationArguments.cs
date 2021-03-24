// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents the runtime representation of the projection registartion arguments.
    /// </summary>
    /// <param name="ExecutionContext">The execution context.</param>
    /// <param name="Projection">The projection id.</param>
    /// <param name="InitialState">The initital state of the projection.</param>
    /// <param name="Events">The event selector mappings.</param>
    /// <param name="Scope">The scope id that the projection should be registered in.</param>
    /// <returns></returns>
    public record ProjectionRegistrationArguments(
        ExecutionContext ExecutionContext,
        EventProcessorId Projection,
        string InitialState,
        IEnumerable<ProjectionEventSelector> Events,
        ScopeId Scope);
}

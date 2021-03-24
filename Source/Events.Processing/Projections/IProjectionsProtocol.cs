// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Defines the protocol for projections.
    /// </summary>
    public interface IProjectionsProtocol : IReverseCallServiceProtocol<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse, ProjectionRegistrationArguments>
    {
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.Projections;

namespace Dolittle.Runtime.Events.Processing.Management.Projections;

public class ProjectionsService : ProjectionsBase
{
    readonly IProjections _projections;

    public ProjectionsService(IProjections projections)
    {
        _projections = projections;
    }

    /// <inheritdoc />
    public override Task<GetAllProjectionsResponse> GetAll(GetAllProjectionsRequest request, ServerCallContext context)
    {
        var response = new GetAllProjectionsResponse();
        response.Projections.AddRange(_projections.All.Select(CreateStatusFromInfo));
        return Task.FromResult(response);
    }

    ProjectionStatus CreateStatusFromInfo(ProjectionInfo info)
    {
        var status = new ProjectionStatus
        {
            Alias = info.Alias,
            // Copies
            // Others
            InitialState = info.Definition.InitialState,
            ScopeId = info.Definition.Scope.ToProtobuf(),
            ProjectionId = info.Definition.Projection.ToProtobuf(),
        };

        return status;
    }
    
    
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Processing.Management.StreamProcessors;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.Projections;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Runtime.Events.Processing.Management.Projections;

public class ProjectionsService : ProjectionsBase
{
    readonly IProjections _projections;
    readonly IConvertProjectionDefinitions _definitionConverter;
    readonly IConvertStreamProcessorStatuses _streamProcessorStatusConverter;

    public ProjectionsService(
        IProjections projections,
        IConvertProjectionDefinitions definitionConverter,
        IConvertStreamProcessorStatuses streamProcessorStatusConverter)
    {
        _projections = projections;
        _definitionConverter = definitionConverter;
        _streamProcessorStatusConverter = streamProcessorStatusConverter;
    }

    /// <inheritdoc />
    public override Task<GetAllProjectionsResponse> GetAll(GetAllProjectionsRequest request, ServerCallContext context)
    {
        var response = new GetAllProjectionsResponse();
        response.Projections.AddRange(_projections.All.Select(_ => CreateStatusFromInfo(_, request.TenantId?.ToGuid())));
        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task<GetOneProjectionResponse> GetOne(GetOneProjectionRequest request, ServerCallContext context)
    {
        var info = _projections.All.FirstOrDefault(projection =>
            projection.Definition.Scope.Value == request.ScopeId.ToGuid() &&
            projection.Definition.Projection.Value == request.ProjectionId.ToGuid());

        var response = new GetOneProjectionResponse();
        
        if (info == default)
        {
            response.Failure = new Failure
            {
                Id = FailureId.Other.ToProtobuf(),
                Reason = $"Projection {request.ProjectionId.ToGuid()} in scope {request.ScopeId.ToGuid()} is not registered",
            };
        }
        else
        {
            response.Projection = CreateStatusFromInfo(info, request.TenantId?.ToGuid());
        }

        return Task.FromResult(response);
    }

    ProjectionStatus CreateStatusFromInfo(ProjectionInfo info, TenantId tenant = null)
    {
        var status = new ProjectionStatus
        {
            Alias = info.Alias,
            Copies = _definitionConverter.ToContractsCopySpecification(info.Definition.Copies),
            InitialState = info.Definition.InitialState,
            ScopeId = info.Definition.Scope.ToProtobuf(),
            ProjectionId = info.Definition.Projection.ToProtobuf(),
        };
        status.Events.AddRange(_definitionConverter.ToContractsEventSelectors(info.Definition.Events));
        status.Tenants.AddRange(CreateScopedStreamProcessorStatus(info, tenant));
        return status;
    }
    
    IEnumerable<TenantScopedStreamProcessorStatus> CreateScopedStreamProcessorStatus(ProjectionInfo info, TenantId tenant = null)
    {
        var state = _projections.CurrentStateFor(info.Definition.Scope, info.Definition.Projection);
        if (!state.Success)
        {
            throw state.Exception;
        }

        return tenant == null
            ? _streamProcessorStatusConverter.Convert(state.Result)
            : _streamProcessorStatusConverter.ConvertForTenant(state.Result, tenant);
    }
}

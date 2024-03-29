// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Processing.Management.StreamProcessors;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.Projections;

namespace Dolittle.Runtime.Events.Processing.Management.Projections;

/// <summary>
/// Represents an implementation of <see cref="ProjectionsBase"/>.
/// </summary>
[ManagementService, ManagementWebService]
public class ProjectionsService : ProjectionsBase
{
    readonly IProjections _projections;
    readonly IExceptionToFailureConverter _exceptionToFailureConverter;
    readonly IConvertProjectionDefinitions _definitionConverter;
    readonly IConvertStreamProcessorStatuses _streamProcessorStatusConverter;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsService"/> class.
    /// </summary>
    /// <param name="projections">The <see cref="IProjections"/> to use to perform operations on Projections.</param>
    /// <param name="exceptionToFailureConverter">The <see cref="IExceptionToFailureConverter"/> to use to convert exceptions to failures.</param>
    /// <param name="definitionConverter">The <see cref="IConvertProjectionDefinitions"/> to use to convert projection definition fields.</param>
    /// <param name="streamProcessorStatusConverter">The <see cref="IConvertStreamProcessorStatuses"/> to use to convert stream processor states.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public ProjectionsService(
        IProjections projections,
        IExceptionToFailureConverter exceptionToFailureConverter,
        IConvertProjectionDefinitions definitionConverter,
        IConvertStreamProcessorStatuses streamProcessorStatusConverter,
        ILogger logger)
    {
        _projections = projections;
        _exceptionToFailureConverter = exceptionToFailureConverter;
        _definitionConverter = definitionConverter;
        _streamProcessorStatusConverter = streamProcessorStatusConverter;
        _logger = logger;
    }

    /// <inheritdoc />
    public override Task<GetAllProjectionsResponse> GetAll(GetAllProjectionsRequest request, ServerCallContext context)
    {
        Log.GetAll(_logger);
        var response = new GetAllProjectionsResponse();
        response.Projections.AddRange(_projections.All.Select(_ => CreateStatusFromInfo(_, request.TenantId?.ToGuid())));
        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task<GetOneProjectionResponse> GetOne(GetOneProjectionRequest request, ServerCallContext context)
    {
        var response = new GetOneProjectionResponse();
        
        var getIds = GetScopeAndProjectionIds(request.ScopeId, request.ProjectionId);
        if (!getIds.Success)
        {
            response.Failure = _exceptionToFailureConverter.ToFailure(getIds.Exception);
            return Task.FromResult(response);
        }
        var (scope, projection) = getIds.Result;
        
        Log.GetOne(_logger, projection, scope);

        var info = _projections.All.FirstOrDefault(_ => _.Definition.Scope == scope && _.Definition.Projection == projection);
        if (info == default)
        {
            Log.ProjectionNotRegistered(_logger, projection, scope);
            response.Failure = _exceptionToFailureConverter.ToFailure(new ProjectionNotRegistered(scope, projection));
            return Task.FromResult(response);
        }
        
        response.Projection = CreateStatusFromInfo(info, request.TenantId?.ToGuid());
        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override async Task<ReplayProjectionResponse> Replay(ReplayProjectionRequest request, ServerCallContext context)
    {
        var response = new ReplayProjectionResponse();
        
        var getIds = GetScopeAndProjectionIds(request.ScopeId, request.ProjectionId);
        if (!getIds.Success)
        {
            response.Failure = _exceptionToFailureConverter.ToFailure(getIds.Exception);
            return response;
        }
        
        var (scope, projection) = getIds.Result;
        
        Log.Replay(_logger, projection, scope);
        
        var result = request.TenantId == null
            ? await _projections.ReplayEventsForAllTenants(scope, projection).ConfigureAwait(false)
            : await _projections.ReplayEventsForTenant(scope, projection, request.TenantId.ToGuid()).ConfigureAwait(false);

        if (!result.Success)
        {
            Log.FailedToReplayProjection(_logger, projection, scope, result.Exception);
            response.Failure = _exceptionToFailureConverter.ToFailure(result.Exception);
        }

        return response;
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
    
    IEnumerable<TenantScopedStreamProcessorStatus> CreateScopedStreamProcessorStatus(ProjectionInfo info, TenantId? tenant = null)
    {
        var state = _projections.CurrentStateFor(info.Definition.Scope, info.Definition.Projection);
        if (!state.Success)
        {
            throw state.Exception;
        }

        if (tenant == null)
        {
            Log.CreatingProjectionStatusForAllTenants(_logger, info.Definition.Projection, info.Definition.Scope);
            return _streamProcessorStatusConverter.Convert(state.Result);
        }
        
        Log.CreatingProjectionStatusForTenant(_logger, info.Definition.Projection, info.Definition.Scope, tenant);
        return _streamProcessorStatusConverter.ConvertForTenant(state.Result, tenant);
    }

    static Try<(ScopeId scopeId,ProjectionId projectionId)> GetScopeAndProjectionIds(Uuid? scope, Uuid? projection)
    {
        if (scope == default)
        {
            return Try<(ScopeId,ProjectionId)>.Failed(new ArgumentNullException(nameof(scope), "Scope id is missing in request"));
        }
        if (projection == default)
        {
            return Try<(ScopeId,ProjectionId)>.Failed(new ArgumentNullException(nameof(projection), "Projection id is missing in request"));
        }
        
        (ScopeId,ProjectionId) result = (scope.ToGuid(), projection.ToGuid());

        return Try<(ScopeId,ProjectionId)>.Succeeded(result);
    }
}

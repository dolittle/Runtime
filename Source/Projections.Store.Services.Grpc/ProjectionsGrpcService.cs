// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Projections.Contracts.Projections;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc
{
    /// <summary>
    /// Represents the implementation of.
    /// </summary>
    public class ProjectionsGrpcService : ProjectionsBase
    {
        readonly IProjectionsService _projectionsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionsGrpcService"/> class.
        /// </summary>
        /// <param name="projectionsService"><see cref="IProjectionsService"/>.</param>
        public ProjectionsGrpcService(
            IProjectionsService projectionsService)
        {
            _projectionsService = projectionsService;
        }

        /// <inheritdoc/>
        public override async Task<Contracts.GetOneResponse> GetOne(Contracts.GetOneRequest request, ServerCallContext context)
        {
            var response = new Contracts.GetOneResponse();
            var getOneResult = await _projectionsService.TryGetOne(
                request.ProjectionId.ToGuid(),
                request.ScopeId.ToGuid(),
                request.Key,
                request.CallContext.ExecutionContext.ToExecutionContext(),
                context.CancellationToken).ConfigureAwait(false);

            if (getOneResult.Success)
                response.State = getOneResult.Result.ToProtobuf();
            else
                response.Failure = getOneResult.Exception.ToFailure();

            return response;
        }

        /// <inheritdoc/>
        public override async Task<Contracts.GetAllResponse> GetAll(Contracts.GetAllRequest request, ServerCallContext context)
        {
            var response = new Contracts.GetAllResponse();
            var getOneResult = await _projectionsService.TryGetAll(
                request.ProjectionId.ToGuid(),
                request.ScopeId.ToGuid(),
                request.CallContext.ExecutionContext.ToExecutionContext(),
                context.CancellationToken).ConfigureAwait(false);

            if (getOneResult.Success)
                response.States.AddRange(getOneResult.Result.ToProtobuf());
            else
                response.Failure = getOneResult.Exception.ToFailure();

            return response;
        }
    }

    public static class ProjectionCurrentStateExtensions
    {
        public static Events.Processing.Contracts.ProjectionCurrentState ToProtobuf(this ProjectionCurrentState state)
            => new()
            {
                Type = state.Type.ToProtobuf(),
                State = state.State
            };
        public static IEnumerable<Events.Processing.Contracts.ProjectionCurrentState> ToProtobuf(this IEnumerable<ProjectionCurrentState> states)
            => states.Select(_ => _.ToProtobuf());

        public static Events.Processing.Contracts.ProjectionCurrentStateType ToProtobuf(this ProjectionCurrentStateType type)
            => type switch
            {
                ProjectionCurrentStateType.CreatedFromInitialState => Events.Processing.Contracts.ProjectionCurrentStateType.CreatedFromInitialState,
                ProjectionCurrentStateType.Persisted => Events.Processing.Contracts.ProjectionCurrentStateType.Persisted
            };
    }

}

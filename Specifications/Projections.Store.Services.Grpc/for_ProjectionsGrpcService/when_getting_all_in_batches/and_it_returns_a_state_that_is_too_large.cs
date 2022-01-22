// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using It = Machine.Specifications.It;
using ProjectionCurrentState = Dolittle.Runtime.Projections.Store.State.ProjectionCurrentState;
using ProjectionCurrentStateType = Dolittle.Runtime.Projections.Store.State.ProjectionCurrentStateType;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc.for_ProjectionsGrpcService.when_getting_all_in_batches;

public class and_it_returns_a_state_that_is_too_large : given.the_service_and_get_all_request
{
    Establish context = () =>
    {
        var states = AsyncEnumerable.Empty<ProjectionCurrentState>()
            .Append(new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, "small state", "key 01"))
            .Append(new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, GenerateRandomString(10000000), "key 02"))
            .Append(new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, "small state", "key 03"));

        projections_service
            .Setup(_ => _.TryGetAll(
                Moq.It.IsAny<ProjectionId>(),
                Moq.It.IsAny<ScopeId>(),
                Moq.It.IsAny<ExecutionContext>(),
                Moq.It.IsAny<CancellationToken>()))
            .ReturnsAsync(Try<IAsyncEnumerable<ProjectionCurrentState>>.Succeeded(states));
    };

    Because of = () => grpc_service.GetAllInBatches(request, server_stream_writer.Object, call_context).GetAwaiter().GetResult();

    It should_send_three_batches_of_one_state = () => server_stream_writer.Verify(_ => _.WriteAsync(Moq.It.Is<GetAllResponse>(response => response.States.Count == 1)), Times.Exactly(3));
    It should_not_send_any_other_messages = () => server_stream_writer.VerifyNoOtherCalls();
}
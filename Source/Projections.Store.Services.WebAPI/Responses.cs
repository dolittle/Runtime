// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Projections.Store.Services.WebAPI
{
    public record FailureResponse(Guid Id, string Reason)
    {
        public static FailureResponse From(Failure failure)
            => new(failure.Id, failure.Reason);
    }
    public record JsonResponseProjectionCurrentState(
        ProjectionCurrentStateType Type,
        string State
    )
    {
        public static JsonResponseProjectionCurrentState From(ProjectionCurrentState currentState)
            => new(currentState.Type, currentState.State);
    }

    public record GetOneResponse(JsonResponseProjectionCurrentState State, FailureResponse Failure)
    {
        public static GetOneResponse From(ProjectionCurrentState state)
            => new(JsonResponseProjectionCurrentState.From(state), null);
        public static GetOneResponse From(Failure failure)
            => new(null, FailureResponse.From(failure));
    }

    public record GetAllResponse(JsonResponseProjectionCurrentState[] States, FailureResponse Failure)
    {
        public static GetAllResponse From(IEnumerable<ProjectionCurrentState> states)
            => new(states.Select(_ => JsonResponseProjectionCurrentState.From(_)).ToArray(), null);
        public static GetAllResponse From(Failure failure)
            => new(null, FailureResponse.From(failure));
    }
}
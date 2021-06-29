// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Projections.Store.State;
using ContractsProjectionCurrentState = Dolittle.Runtime.Projections.Contracts.ProjectionCurrentState;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Extensions for <see cref="ProjectionCurrentState"/>.
    /// </summary>
    public static class ProjectionCurrentStateExtensions
    {
        /// <summary>
        /// Convert to a protobuf representation of <see cref="ProjectionCurrentState"/>.
        /// </summary>
        /// <param name="state"><see cref="ProjectionCurrentState"/> to convert.</param>
        /// <returns>Converted <see cref="ContractsProjectionCurrentState"/>.</returns>
        public static ContractsProjectionCurrentState ToProtobuf(this ProjectionCurrentState state)
            => new()
            {
                Type = state.Type.ToProtobuf(),
                State = state.State,
                Key = state.Key
            };

        /// <summary>
        /// Convert to an <see cref="IEnumerable{T}"/> of protobuf representations of <see cref="ProjectionCurrentState"/>.
        /// </summary>
        /// <param name="states">The <see cref="IEnumerable{T}"/> of type <see cref="ProjectionCurrentState"/> to convert.</param>
        /// <returns>Converted <see cref="IEnumerable{T}"/> of type <see cref="ContractsProjectionCurrentState"/>.</returns>
        public static IEnumerable<ContractsProjectionCurrentState> ToProtobuf(this IEnumerable<ProjectionCurrentState> states)
            => states.Select(_ => _.ToProtobuf());
    }
}

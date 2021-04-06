// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ContractsProjectionCurrentStateType = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCurrentStateType;

namespace Dolittle.Runtime.Projections.Store.Services
{
    /// <summary>
    /// Extensions for <see cref="ProjectionCurrentStateType"/>.
    /// </summary>
    public static class ProjectionCurrentStateTypeExtensions
    {
        /// <summary>
        /// Convert to a protobuf representation of <see cref="ProjectionCurrentStateType"/>.
        /// </summary>
        /// <param name="type"><see cref="ProjectionCurrentStateType"/> to convert.</param>
        /// <returns>Converted <see cref="ContractsProjectionCurrentStateType"/>.</returns>
        public static ContractsProjectionCurrentStateType ToProtobuf(this ProjectionCurrentStateType type)
            => type switch
            {
                ProjectionCurrentStateType.CreatedFromInitialState => ContractsProjectionCurrentStateType.CreatedFromInitialState,
                ProjectionCurrentStateType.Persisted => ContractsProjectionCurrentStateType.Persisted,
                _ => throw new UnknownProjectionCurrentStateType(type),
            };
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.State
{
    /// <summary>
    /// Represents the different current projection state types
    /// </summary>
    public enum ProjectionCurrentStateType : ushort
    {
        /// <summary>
        /// The type when the projection state is created from initial state.
        /// </summary>
        CreatedFromInititalState = 0,

        /// <summary>
        /// The type when the projection state has already been persisted.
        /// </summary>
        Persisted
    }
}

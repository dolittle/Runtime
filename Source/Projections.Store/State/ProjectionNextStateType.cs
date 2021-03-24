// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.State
{
    /// <summary>
    /// Represents the different next state types.
    /// </summary>
    public enum ProjectionNextStateType : ushort
    {
        Replace = 0,
        Delete
    }
}

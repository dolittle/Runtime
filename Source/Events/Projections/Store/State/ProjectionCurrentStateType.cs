// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.State;

public enum ProjectionCurrentStateType : ushort
{
    CreatedFromInitialState = 0,
    Persisted
}
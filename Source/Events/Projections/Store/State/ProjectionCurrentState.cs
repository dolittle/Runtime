// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.State;

/// <summary>
/// The current projection state.
/// </summary>
/// <param name="Type">The type of the state.</param>
/// <param name="State">The state.</param>
/// <param name="Key">The key of the projection.</param>
public record ProjectionCurrentState(ProjectionCurrentStateType Type, ProjectionState State, ProjectionKey Key);
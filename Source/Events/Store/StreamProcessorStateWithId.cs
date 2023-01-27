// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents both the <see cref="IStreamProcessorState"/> with its corresponding <see cref="IStreamProcessorId"/>.
/// </summary>
/// <param name="Id">The <see cref="IStreamProcessorId"/>.</param>
/// <param name="State">The <see cref="IStreamProcessorState"/>.</param>
public record StreamProcessorStateWithId<TId, TState>(TId Id, TState State)
    where TId : IStreamProcessorId
    where TState : IStreamProcessorState;

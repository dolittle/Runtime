// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Exception that gets thrown when attempting to set a stream processor position that is higher than the last processed position. 
/// </summary>
public class CannotSetStreamProcessorPositionHigherThanCurrentPosition : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotSetStreamProcessorPositionHigherThanCurrentPosition"/> class.
    /// </summary>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState"/>.</param>
    /// <param name="position">The new <see cref="StreamPosition"/>.</param>
    public CannotSetStreamProcessorPositionHigherThanCurrentPosition(IStreamProcessorId streamProcessorId, IStreamProcessorState currentState, StreamPosition position)
        : base($"Stream Processor: '{streamProcessorId}' cannot be set to new position {position} because it is already at position {currentState.Position}")
    {
    }
}

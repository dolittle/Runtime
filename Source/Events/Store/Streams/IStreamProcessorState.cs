// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Actors;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines the basis for the state of a <see cref="AbstractScopedStreamProcessor" />.
/// </summary>
public interface IStreamProcessorState
{
    /// <summary>
    /// Gets the <see cref="ProcessingPosition" />.
    /// </summary>
    ProcessingPosition Position { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="AbstractScopedStreamProcessor" /> is partitioned or not.
    /// </summary>
    bool Partitioned { get; }

    Bucket ToProtobuf();
    
    /// <summary>
    ///  Get the earliest position that the stream processor needs to process. This is normally the same as the <see cref="Position"/>
    /// but can be earlier in the stream if the stream processor is partitioned and there exists failing partitions.
    /// </summary>
    ProcessingPosition EarliestPosition { get; }
}

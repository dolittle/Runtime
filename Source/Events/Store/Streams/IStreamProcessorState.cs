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
    /// Gets the <see cref="StreamPosition" />.
    /// </summary>
    StreamPosition Position { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="AbstractScopedStreamProcessor" /> is partitioned or not.
    /// </summary>
    bool Partitioned { get; }

    Bucket ToProtobuf();
}

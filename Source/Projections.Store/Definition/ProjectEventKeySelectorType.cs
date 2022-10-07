// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.Definition;

/// <summary>
/// Represents the event key selector types for projections.
/// </summary>
public enum ProjectEventKeySelectorType : ushort
{
    /// <summary>
    /// The type when <see cref="Events.EventSourceId" /> is the key.
    /// </summary>
    EventSourceId = 0,

    /// <summary>
    /// The type when <see cref="Events.Store.Streams.PartitionId" /> is the key.
    /// </summary>
    PartitionId,

    /// <summary>
    /// The type when a specific property on the event is the key.
    /// </summary>
    Property,
        
    /// <summary>
    /// The type when a constant, static, string is the key.
    /// </summary>
    Static,
    
    /// <summary>
    /// The type when the date time is the key.
    /// </summary>
    EventOccurred
}

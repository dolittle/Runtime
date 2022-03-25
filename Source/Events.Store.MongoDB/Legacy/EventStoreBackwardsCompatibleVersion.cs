// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Represents the version of the Event Store schema to be compatible with.
/// </summary>
public enum EventStoreBackwardsCompatibleVersion
{
    /// <summary>
    /// The default configured value, where compatibility has not been selected.
    /// </summary>
    /// <remarks>
    /// If this value is used, all use of the Event Store will crash at Runtime - as we cannot decide which version to be backwards compatible with dynamically.
    /// </remarks>
    NotSet,
    
    /// <summary>
    /// Configures the Event Store persistence to be backwards compatible with the v6 Runtime as far as possible.
    /// Meaning that EventSourceIds and PartitionIds will be persisted as Guids when the string is convertible to a Guid.
    /// </summary>
    V6,
    
    /// <summary>
    /// Configures the Event Store persistence to be backwards compatible with the v7 Runtime.
    /// EventSourceIds and PartitionIds will always be persisted as strings.
    /// </summary>
    V7,
}

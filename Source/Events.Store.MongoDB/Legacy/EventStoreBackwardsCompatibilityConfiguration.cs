// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Represents the configuration of Event Store backwards compatibility.
/// </summary>
/// <param name="Version">The previous version of the Runtime to be backwards compatible with.</param>
[Configuration("eventStore:backwardsCompatibility")]
public record EventStoreBackwardsCompatibilityConfiguration(
    EventStoreBackwardsCompatibleVersion Version = EventStoreBackwardsCompatibleVersion.NotSet);

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Defines a system that configures the MongoDB driver so that the Event Store schema is backwards compatible with a previous Runtime release.
/// </summary>
public interface IConfigureBackwardsCompatibility
{
    /// <summary>
    /// Configures the MongoDB serializers for backwards compatibility.
    /// </summary>
    void ConfigureSerializers();
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Represents an implementation of <see cref="IConfigureBackwardsCompatibility"/>.
/// </summary>
[Singleton]
public class BackwardsCompatibility : IConfigureBackwardsCompatibility
{
    readonly EventStoreBackwardsCompatibilityConfiguration _configuration;
    static readonly object _configurationLock = new();
    static bool _serializersConfigured;

    public BackwardsCompatibility(IOptions<EventStoreBackwardsCompatibilityConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    /// <inheritdoc />
    public void ConfigureSerializers()
    {
        if (_serializersConfigured)
        {
            return;
        }
        
        lock (_configurationLock)
        {
            if (_serializersConfigured)
            {
                return;
            }

            BsonSerializer.RegisterSerializer(new EventSourceAndPartitionSerializer(_configuration.Version));

            _serializersConfigured = true;
        }
    }
}

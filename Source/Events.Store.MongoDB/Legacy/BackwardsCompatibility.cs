// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Represents an implementation of <see cref="IConfigureBackwardsCompatibility"/>.
/// </summary>
[Singleton]
public class BackwardsCompatibility : IConfigureBackwardsCompatibility
{
    readonly ConventionPack _eventSourceAndPartitionConventions;
    static readonly object _configurationLock = new();
    static bool _serializersConfigured;

    public BackwardsCompatibility(IOptions<EventStoreBackwardsCompatibilityConfiguration> configuration)
    {
        var serializer = new EventSourceAndPartitionSerializer(configuration.Value.Version);

        _eventSourceAndPartitionConventions = new ConventionPack();
        _eventSourceAndPartitionConventions.AddClassMapConvention("EventSource and Partition backwards compatibility", cm =>
        {
            if (cm.GetMemberMap("EventSource") is {} eventSourceMemberMap && eventSourceMemberMap.MemberType == typeof(string))
            {
                eventSourceMemberMap.SetSerializer(serializer);
            }
            if (cm.GetMemberMap("Partition") is {} partitionMemberMap && partitionMemberMap.MemberType == typeof(string))
            {
                partitionMemberMap.SetSerializer(serializer);
            }
        });
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

            ConventionRegistry.Register(
                "EventSource and Partition backwards compatibility", 
                _eventSourceAndPartitionConventions, 
                _ => _.Namespace?.StartsWith("Dolittle.Runtime.Events.Store.MongoDB", StringComparison.InvariantCulture) ?? false);

            _serializersConfigured = true;
        }
    }
}

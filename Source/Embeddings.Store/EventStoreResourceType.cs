// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Represents a <see cref="IAmAResourceType">resource type</see> for an event store.
/// </summary>
public class EventStoreResourceType : IAmAResourceType
{
    /// <inheritdoc/>
    public ResourceType Name => "embeddings";

    /// <inheritdoc/>
    public IEnumerable<Type> Services { get; } = new[]
    {
        typeof(IEmbeddingStates),
        typeof(IEmbeddingDefinitions)
    };
}
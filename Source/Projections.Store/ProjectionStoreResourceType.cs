// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Represents a <see cref="IAmAResourceType">resource type</see> for a projections store.
/// </summary>
public class ProjectionStoreResourceType : IAmAResourceType
{
    /// <inheritdoc/>
    public ResourceType Name => "projections";

    /// <inheritdoc/>
    public IEnumerable<Type> Services { get; } = new[]
    {
        typeof(IProjectionStates),
        typeof(IProjectionDefinitions),
    };
}
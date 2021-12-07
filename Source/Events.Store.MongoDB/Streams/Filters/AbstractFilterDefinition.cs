// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;

/// <summary>
/// Represents a persisted <see cref="Store.Streams.Filters.IFilterDefinition" />.
/// </summary>
public abstract class AbstractFilterDefinition
{
    /// <summary>
    /// Converts the stored filter into the runtime <see cref="IFilterDefinition" /> that it represents.
    /// </summary>
    /// <param name="streamId">The Stream Id.</param>
    /// <param name="partitioned">Whether or not the stream definition is partitioned.</param>
    /// <param name="public">Whether or not the stream definition is public.</param>
    /// <returns>The runtime <see cref="IFilterDefinition" />.</returns>
    public abstract IFilterDefinition AsRuntimeRepresentation(Guid streamId, bool partitioned, bool @public);
}
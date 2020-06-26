// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="Store.Streams.Filters.FilterDefinition" />.
    /// </summary>
    public class RemoteFilterDefinition : AbstractFilterDefinition
    {
        /// <inheritdoc/>
        public override IFilterDefinition AsRuntimeRepresentation(Guid streamId, bool partitioned, bool @public)
        {
            if (@public)
            {
                return new PublicFilterDefinition(StreamId.EventLog, streamId);
            }
            else
            {
                return new FilterDefinition(StreamId.EventLog, streamId, partitioned);
            }
        }
    }
}

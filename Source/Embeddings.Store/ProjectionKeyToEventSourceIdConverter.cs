// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Data.HashFunction;
using System.Data.HashFunction.CityHash;
using System.Text;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IConvertProjectionKeysToEventSourceIds"/>.
    /// </summary>
    public class ProjectionKeyToEventSourceIdConverter : IConvertProjectionKeysToEventSourceIds
    {
        /// <inheritdoc/>
        public EventSourceId GetEventSourceIdFor(ProjectionKey key)
        {
            return key.Value;
        }
    }
}

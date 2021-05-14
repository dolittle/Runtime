// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Defines a system that can convert <see cref="ProjectionKey"/> to <see cref="EventSourceId"/>.
    /// </summary>
    public interface IConvertProjectionKeysToEventSourceIds
    {
        /// <summary>
        /// Gets the correspoing event source id for the given projection key.
        /// </summary>
        /// <param name="key">The <see cref="ProjectionKey"/> to convert.</param>
        /// <returns>The <see cref="EventSourceId"/> that corresponds to the provided key.</returns>
        EventSourceId GetEventSourceIdFor(ProjectionKey key);
    }
}

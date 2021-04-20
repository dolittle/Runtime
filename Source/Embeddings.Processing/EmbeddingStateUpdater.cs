// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IUpdateEmbeddingStates"/>.
    /// </summary>
    public class EmbeddingStateUpdater : IUpdateEmbeddingStates
    {
        readonly EmbeddingId _embedding;
        readonly IProjection _projection;
        readonly IEventStore _eventStore;
        readonly IEmbeddingStore _embeddingStore;
        readonly IConvertProjectionKeysToEventSourceIds _keyToEventSourceConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingStateUpdater"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> that identifies the embedding.</param>
        /// <param name="projection">The <see cref="IProjection"/> that is used to update the state.</param>
        /// <param name="eventStore">The <see cref="IEventStore"/> that is used to fetch aggregate events.</param>
        /// <param name="embeddingStore">The <see cref="IEmbeddingStore"/> that is used to persist the states.</param>
        /// <param name="keyToEventSourceConverter">The <see cref="IConvertProjectionKeysToEventSourceIds"/> to use for converting projection keys to event source ids.</param>
        public EmbeddingStateUpdater(
            EmbeddingId embedding,
            IProjection projection,
            IEventStore eventStore,
            IEmbeddingStore embeddingStore, IConvertProjectionKeysToEventSourceIds keyToEventSourceConverter)
        {
            _embedding = embedding;
            _projection = projection;
            _eventStore = eventStore;
            _embeddingStore = embeddingStore;
            _keyToEventSourceConverter = keyToEventSourceConverter;
        }

        /// <inheritdoc/>
        public Task<Try> TryUpdateAll(CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}
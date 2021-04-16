// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingProcessor"/>.
    /// </summary>
    public class EmbeddingProcessor : IEmbeddingProcessor
    {
        readonly EmbeddingId _embedding;
        readonly IUpdateEmbeddingStates _stateUpdater;
        readonly IWaitForAggregateRootEvents _eventWaiter;
        readonly IEventStore _eventStore;
        readonly IEmbeddingStore _embeddingStore;
        readonly ICalculateStateTransistionEvents _transitionCalculator;
        CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingProcessor"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> that identifies the embedding.</param>
        /// <param name="stateUpdater">The <see cref="IUpdateEmbeddingStates"/> to use for recalculating embedding states from aggregate root events.</param>
        /// <param name="eventWaiter">The <see cref="IWaitForAggregateRootEvents"/> to use for waiting on aggregate root events to be committed.</param>
        /// <param name="eventStore">The <see cref="IEventStore"/> to use for fetching and committing aggregate root events.</param>
        /// <param name="embeddingStore">The <see cref="IEmbeddingStore"/> to use for fetching, replacing and removing embedding states.</param>
        /// <param name="transitionCalculator">The <see cref="ICalculateStateTransistionEvents"/> to use for calculating state transition events.</param>
        public EmbeddingProcessor(
            EmbeddingId embedding,
            IUpdateEmbeddingStates stateUpdater,
            IWaitForAggregateRootEvents eventWaiter,
            IEventStore eventStore,
            IEmbeddingStore embeddingStore,
            ICalculateStateTransistionEvents transitionCalculator)
        {
            _embedding = embedding;
            _stateUpdater = stateUpdater;
            _eventWaiter = eventWaiter;
            _eventStore = eventStore;
            _embeddingStore = embeddingStore;
            _transitionCalculator = transitionCalculator;
        }

        /// <inheritdoc/>
        public Task Start(CancellationToken cancellationToken)
        {
            ThrowIfAlreadyStarted();
            _cancellationToken = cancellationToken;
            return Task.Run(Loop);
        }

        /// <inheritdoc/>
        public Task<Try<ProjectionState>> Update(ProjectionKey key, ProjectionState state, CancellationToken cancellationToken) => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public Task<Try> Delete(ProjectionKey key, CancellationToken cancellationToken) => throw new System.NotImplementedException();

        async Task Loop()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                EnsureAllStatesAreFresh();

                // await _eventWaiter.WaitForEvent(_embedding, ).ConfigureAwait(false);
                // Wait for any events to be applied to the aggregate root, or an incoming update/delete request

                // If update/delete request - handle it
            }
        }

        void EnsureAllStatesAreFresh() => throw new System.NotImplementedException();

        void ThrowIfAlreadyStarted() => throw new System.NotImplementedException();
        void ThrowIfNotStarted() => throw new System.NotImplementedException();
    }
}
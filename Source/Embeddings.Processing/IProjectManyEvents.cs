// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a system that can project many events.
    /// </summary>
    public interface IProjectManyEvents
    {
        /// <summary>
        /// Tries to project all the <see cref="CommittedAggregateEvents" />.
        /// </summary>
        /// <param name="currentState">The <see cref="EmbeddingCurrentState" />.</param>
        /// <param name="events">The <see cref="CommittedAggregateEvents" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns></returns>
        Task<Partial<EmbeddingCurrentState>> TryProject(EmbeddingCurrentState currentState, CommittedAggregateEvents events, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to project all the <see cref="UncommittedEvents" />.
        /// </summary>
        /// <param name="currentState">The <see cref="EmbeddingCurrentState" />.</param>
        /// <param name="events">The <see cref="UncommittedEvents" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns></returns>
        Task<Partial<EmbeddingCurrentState>> TryProject(EmbeddingCurrentState currentState, UncommittedEvents events, CancellationToken cancellationToken);
    }
}
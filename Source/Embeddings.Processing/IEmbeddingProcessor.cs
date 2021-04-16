// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    public interface IEmbeddingProcessor
    {
        /// <summary>
        /// Starts the embedding processing.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to signal the started processor to stop.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="Try" /> that indicates whether the processor completes or fails.</returns>
        Task<Try> Start(CancellationToken cancellationToken);

        /// <summary>
        /// Tries to update embedding to match the projected state with the given state for a key.
        /// </summary>
        /// <param name="key">The <see cref="ProjectionKey"/> that should be updated.</param>
        /// <param name="state">The desired <see cref="ProjectionState"/> to reach.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> that is resolved to a <see cref="ProjectionState"/> when the operation completes.</returns>
        Task<Try<ProjectionState>> Update(ProjectionKey key, ProjectionState state, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to delete the embedding state for a key.
        /// </summary>
        /// <param name="key">The <see cref="ProjectionKey"/> that should be deleted.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> that is resolved to a <see cref="Try"/> result.</returns>
        Task<Try> Delete(ProjectionKey key, CancellationToken cancellationToken);
    }
}
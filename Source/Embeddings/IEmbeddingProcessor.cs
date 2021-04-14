// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Embeddings
{
    public interface IEmbeddingProcessor
    {
        /// <summary>
        /// Starts the embedding processing.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to signal the started processor to stop.</param>
        /// <returns>A <see cref="Task"/> that is resolved when the processor completes or fails.</returns>
        Task Start(CancellationToken cancellationToken);
    }
}
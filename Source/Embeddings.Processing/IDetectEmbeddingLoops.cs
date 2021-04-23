// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a system that can detect loops in the logic of an <see cref="IEmbedding"/>.
    /// </summary>
    public interface IDetectEmbeddingLoops
    {
        /// <summary>
        /// Tries to check a set of uncommitted events for loops.
        /// </summary>
        /// <param name="events">The <see cref="IEnumerable{T}"/> of <see cref="UncommittedEvents"/> from an embedding.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of true if it contains loops, false if not.</returns>
        Task<Try<bool>> TryCheckEventLoops(IEnumerable<UncommittedEvents> events);
    }
}

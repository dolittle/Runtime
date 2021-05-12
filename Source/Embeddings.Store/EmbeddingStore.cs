// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Represents the implementation of <see cref="IEmbeddingStore"/>
    /// </summary>
    public class EmbeddingStore : IEmbeddingStore
    {
        public Task<Try<EmbeddingCurrentState>> TryGet(EmbeddingId embedding, ProjectionKey key, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        public Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        public Task<Try> TryRemove(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        public Task<Try> TryReplace(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state, CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}

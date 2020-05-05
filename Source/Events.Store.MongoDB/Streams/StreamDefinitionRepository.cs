// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamDefinitionRepository" />.
    /// </summary>
    public class StreamDefinitionRepository : IStreamDefinitionRepository
    {
        public Task Persist(ScopeId scope, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Try<IStreamDefinition>> TryGet(ScopeId scope, StreamId stream, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
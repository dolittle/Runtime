// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamDefinitions" />.
    /// </summary>
    [SingletonPerTenant]
    public class StreamDefinitions : IStreamDefinitions
    {
        readonly IStreamDefinitionRepository _streamDefinitionRepository;
        readonly ILogger<StreamDefinitions> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDefinitions"/> class.
        /// </summary>
        /// <param name="streamDefinitionRepository">The <see cref="IStreamDefinitionRepository" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamDefinitions(
            IStreamDefinitionRepository streamDefinitionRepository,
            ILogger<StreamDefinitions> logger)
        {
            _streamDefinitionRepository = streamDefinitionRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<StreamDefinition> GetFor(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken)
        {
            if (IsEventLogStream(scopeId, streamId)) return Task.FromResult(StreamDefinition.EventLog);
            return _streamDefinitionRepository.GetFor(scopeId, streamId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> HasFor(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken)
        {
            if (IsEventLogStream(scopeId, streamId)) return Task.FromResult(true);
            return _streamDefinitionRepository.HasFor(scopeId, streamId, cancellationToken);
        }

        bool IsEventLogStream(ScopeId scopeId, StreamId sourceStreamId) => scopeId == ScopeId.Default && sourceStreamId == StreamId.AllStreamId;
    }
}
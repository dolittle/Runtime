// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    [SingletonPerTenant]
    public class Filters : IFilters
    {
        readonly ConcurrentDictionary<StreamId, (IFilterDefinition, AbstractFilterProcessor)> _streamFilterMap = new ConcurrentDictionary<StreamId, (IFilterDefinition, AbstractFilterProcessor)>();
        readonly IFilterValidator _filterValidator;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="filterValidator">The <see cref="IFilterValidator" />.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public Filters(IFilterValidator filterValidator, ILogger logger)
        {
            _logger = logger;
            _filterValidator = filterValidator;
        }

        /// <inheritdoc/>
        public AbstractFilterProcessor GetFilterProcessorFor(StreamId targetStream)
        {
            if (!_streamFilterMap.TryGetValue(targetStream, out var filterAndDefinition)) throw new NoFilterRegisteredForStream(targetStream);
            return filterAndDefinition.Item2;
        }

        /// <inheritdoc/>
        public Task Register(IFilterDefinition definition, AbstractFilterProcessor filter, CancellationToken cancellationToken)
        {
            if (_streamFilterMap.ContainsKey(definition.TargetStream)) throw new FilterForStreamAlreadyRegistered(definition.TargetStream);
            _streamFilterMap.TryAdd(definition.TargetStream, (definition, filter));
            return _filterValidator.Validate(definition, cancellationToken);
        }

        /// <inheritdoc/>
        public void Remove(StreamId sourceStream, StreamId targetStream)
        {
            if (!_streamFilterMap.ContainsKey(targetStream)) throw new NoFilterRegisteredForStream(targetStream);
            _streamFilterMap.TryRemove(targetStream, out var _);
        }
    }
}
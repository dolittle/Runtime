// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterRegistry" />.
    /// </summary>
    [SingletonPerTenant]
    public class FilterRegistry : IFilterRegistry
    {
        readonly ConcurrentDictionary<StreamId, IEventProcessor> _registeredFilters = new ConcurrentDictionary<StreamId, IEventProcessor>();
        readonly IFilterValidators _filterValidators;
        readonly IContainer _container;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistry"/> class.
        /// </summary>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="container">The <see cref="IContainer" />.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public FilterRegistry(IFilterValidators filterValidators, IContainer container, ILogger logger)
        {
            _filterValidators = filterValidators;
            _container = container;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Register<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken = default)
            where TDefinition : IFilterDefinition
        {
            if (!_registeredFilters.TryAdd(filter.Definition.TargetStream, filter)) throw new FilterForStreamAlreadyRegistered(filter.Definition.TargetStream);
            await _filterValidators.Validate(filter, cancellationToken).ConfigureAwait(false);
            var repository = _container.Get<IFilterDefinitionRepositoryFor<TDefinition>>();
            if (repository != null) await repository.PersistFilter(filter.Definition, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public void Unregister(StreamId targetStream)
        {
            if (!_registeredFilters.ContainsKey(targetStream)) throw new NoFilterRegisteredForStream(targetStream);
            _registeredFilters.Remove(targetStream, out var _);
        }

        /// <inheritdoc/>
        public Task RemoveIfPersisted(StreamId targetStream, CancellationToken cancellationToken = default)
        {
            if (!_registeredFilters.TryGetValue(targetStream, out var filterProcessor)) throw new NoFilterRegisteredForStream(targetStream);
            var repository = GetPersistedFilterDefinitionRemoverFromFilterProcessor(filterProcessor);
            if (repository != null) return repository.RemovePersistedFilter(targetStream, cancellationToken);
            return Task.CompletedTask;
        }

        ICanRemovePersistedFilterDefinition GetPersistedFilterDefinitionRemoverFromFilterProcessor(IEventProcessor processor) =>
            GetPersistedFilterDefinitionRemoverForDefinitionType(
                GetFilterDefinitionTypeFromEventProcessor(processor));

        Type GetFilterDefinitionTypeFromEventProcessor(IEventProcessor processor) =>
            processor.GetType().GetGenericArguments().First();

        ICanRemovePersistedFilterDefinition GetPersistedFilterDefinitionRemoverForDefinitionType(Type filterDefinitionType) =>
            _container.Get(typeof(IFilterDefinitionRepositoryFor<>).MakeGenericType(filterDefinitionType)) as ICanRemovePersistedFilterDefinition;
    }
}
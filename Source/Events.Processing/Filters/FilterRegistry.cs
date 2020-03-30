// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
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
            _logger.Debug($"Registering filter defintion of type {typeof(TDefinition).FullName} on source stream '{filter.Definition.SourceStream}' and target stream '{filter.Definition.TargetStream}'");
            if (!_registeredFilters.TryAdd(filter.Definition.TargetStream, filter)) throw new FilterForStreamAlreadyRegistered(filter.Definition.TargetStream);
            await _filterValidators.Validate(filter, cancellationToken).ConfigureAwait(false);
            IFilterDefinitionRepositoryFor<TDefinition> repository = null;
            try
            {
                repository = _container.Get<IFilterDefinitionRepositoryFor<TDefinition>>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"No persisted filter repository for filter definition of type {typeof(TDefinition).FullName}");
            }

            if (repository != null)
            {
                _logger.Debug($"Persisting filter defintion of type {typeof(TDefinition).FullName} on source stream '{filter.Definition.SourceStream}' and target stream '{filter.Definition.TargetStream}'");
                await repository.PersistFilter(filter.Definition, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public void Unregister(ScopeId scope, StreamId targetStream)
        {
            _logger.Debug($"Unregistering filter on target stream '{targetStream}'");
            if (!_registeredFilters.ContainsKey(targetStream)) throw new NoFilterRegisteredForStream(scope, targetStream);
            _registeredFilters.Remove(targetStream, out var _);
        }

        /// <inheritdoc/>
        public Task RemoveIfPersisted(ScopeId scope, StreamId targetStream, CancellationToken cancellationToken = default)
        {
            _logger.Debug($"Removing persisted filter definition on target stream '{targetStream}' if persisted");
            if (!_registeredFilters.TryGetValue(targetStream, out var filterProcessor)) throw new NoFilterRegisteredForStream(scope, targetStream);
            var filterDefinitionType = GetFilterDefinitionTypeFromEventProcessor(filterProcessor);
            ICanRemovePersistedFilterDefinition persistedFilterRemover;
            try
            {
                persistedFilterRemover = GetPersistedFilterDefinitionRemoverForDefinitionType(filterDefinitionType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"No filter definition repository for filter definition of type '{filterDefinitionType.FullName}'");
                return Task.CompletedTask;
            }

            if (persistedFilterRemover != null)
            {
                _logger.Debug($"Removing persisted filter defintion of type '{filterDefinitionType.FullName}' on target stream '{targetStream}'");
                return persistedFilterRemover.RemovePersistedFilter(targetStream, cancellationToken);
            }

            _logger.Debug($"No filter definition repository for filter definition of type '{filterDefinitionType.FullName}'");
            return Task.CompletedTask;
        }

        Type GetFilterDefinitionTypeFromEventProcessor(IEventProcessor processor) =>
            processor.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).First(_ => typeof(IFilterDefinition).IsAssignableFrom(_.PropertyType)).PropertyType;

        ICanRemovePersistedFilterDefinition GetPersistedFilterDefinitionRemoverForDefinitionType(Type filterDefinitionType) =>
            _container.Get(typeof(IFilterDefinitionRepositoryFor<>).MakeGenericType(filterDefinitionType)) as ICanRemovePersistedFilterDefinition;
    }
}
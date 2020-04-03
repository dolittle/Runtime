// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
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
        readonly IFilterDefinitionRepository _filterDefinitions;
        readonly IContainer _container;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistry"/> class.
        /// </summary>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="filterDefinitions">The <see cref="IFilterDefinitionRepository" />.</param>
        /// <param name="container">The <see cref="IContainer" />.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public FilterRegistry(IFilterValidators filterValidators, IFilterDefinitionRepository filterDefinitions, IContainer container, ILogger logger)
        {
            _filterValidators = filterValidators;
            _filterDefinitions = filterDefinitions;
            _container = container;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Register<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken)
            where TDefinition : IFilterDefinition
        {
            _logger.Trace($"Registering filter defintion of type {typeof(TDefinition).FullName} on source stream '{filter.Definition.SourceStream}' and target stream '{filter.Definition.TargetStream}'");
            if (!_registeredFilters.TryAdd(filter.Definition.TargetStream, filter))
            {
                var message = $"Filter '{filter.Definition.TargetStream}' is already registered.";
                _logger.Debug(message);
                throw new CouldNotRegisterFilter(filter.Definition, message);
            }

            var validationResult = await _filterValidators.Validate(filter, cancellationToken).ConfigureAwait(false);
            if (!validationResult.Succeeded)
            {
                var message = $"Filter '{filter.Definition.TargetStream}' failed validation: {validationResult.FailureReason}";
                _logger.Debug(message);
                throw new CouldNotRegisterFilter(filter.Definition, message);
            }

            if (filter.Definition.IsPersistable)
            {
                _logger.Trace($"Persisting filter defintion of type {typeof(TDefinition).FullName} on source stream '{filter.Definition.SourceStream}' and target stream '{filter.Definition.TargetStream}'");
                await _filterDefinitions.PersistFilter(filter.Definition, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public void Unregister(StreamId targetStream)
        {
            _logger.Trace($"Unregistering filter on target stream '{targetStream}'");
            _registeredFilters.Remove(targetStream, out var _);
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterValidators" />.
    /// </summary>
    [Singleton]
    public class FilterValidators : IFilterValidators
    {
        readonly ITypeFinder _typeFinder;
        readonly IContainer _container;
        readonly ILogger _logger;
        readonly IDictionary<Type, Type> _filterDefinitionToValidatorMap = new Dictionary<Type, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidators"/> class.
        /// </summary>
        /// <param name="typeFinder">The <see cref="ITypeFinder" />.</param>
        /// <param name="container">The <see cref="IContainer" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public FilterValidators(ITypeFinder typeFinder, IContainer container, ILogger logger)
        {
            _typeFinder = typeFinder;
            _container = container;
            _logger = logger;
            PopulateFilterValidatorMap();
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate<TDefinition>(IFilterDefinition persistedDefinition, IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken)
            where TDefinition : IFilterDefinition
        {
            _logger.FindingFilterValidator(filter.Identifier);

            if (FilterDefinitionTypeHasChanged(persistedDefinition, filter.Definition))
            {
                return Task.FromResult(new FilterValidationResult("Filter definition type has changed"));
            }

            if (TryGetValidatorFor<TDefinition>(out var validator))
            {
                _logger.ValidatingFilter(filter.Identifier);
                return validator.Validate((TDefinition)persistedDefinition, filter, cancellationToken);
            }

            return Task.FromResult(new FilterValidationResult($"No available filter validator for type {filter.Definition.GetType()}"));
        }

        void PopulateFilterValidatorMap()
        {
            _typeFinder.FindMultiple<IFilterDefinition>().ForEach(filterDefinitionType =>
            {
                if (TryGetValidatorTypeFor(filterDefinitionType, out var validatorType))
                {
                    _logger.FoundValidatorForFilter(filterDefinitionType, validatorType);
                    _filterDefinitionToValidatorMap.TryAdd(filterDefinitionType, validatorType);
                }
            });
        }

        bool FilterDefinitionTypeHasChanged<TDefinition>(IFilterDefinition persitedDefiniton, TDefinition registeredDefinition)
            => persitedDefiniton.GetType() != registeredDefinition.GetType();

        bool TryGetValidatorTypeFor(Type filterDefinitionType, out Type validatorType)
        {
            var implementations = _typeFinder.FindMultiple(typeof(ICanValidateFilterFor<>).MakeGenericType(filterDefinitionType));
            if (implementations.Any())
            {
                if (implementations.Count() > 1)
                {
                    _logger.MultipleValidatorsForFilter(
                        filterDefinitionType,
                        implementations);
                }

                validatorType = implementations.First();
                return true;
            }

            validatorType = null;
            return false;
        }

        bool TryGetValidatorFor<TDefinition>(out ICanValidateFilterFor<TDefinition> validator)
            where TDefinition : IFilterDefinition
        {
            if (_filterDefinitionToValidatorMap.TryGetValue(typeof(TDefinition), out var validatorType))
            {
                validator = _container.Get(validatorType) as ICanValidateFilterFor<TDefinition>;
                return true;
            }

            validator = null;
            return false;
        }
    }
}

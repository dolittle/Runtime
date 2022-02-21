// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System;
//using System.Collections.Generic;
//
//using System.Threading;
//using System.Threading.Tasks;
//using Dolittle.Runtime.DependencyInversion;
//using Dolittle.Runtime.DependencyInversion.Lifecycle;
//using Dolittle.Runtime.Events.Store.Streams.Filters;
//
//using Microsoft.Extensions.Logging;
//using Dolittle.Runtime.Rudimentary;
//using Dolittle.Runtime.Execution;
//using Dolittle.Runtime.Events.Processing.Streams;
//using Dolittle.Runtime.Events.Store.Streams;
//
//namespace Dolittle.Runtime.Events.Processing.Filters;
//
///// <summary>
///// Represents an implementation of <see cref="IFilterValidators" />.
///// </summary>
//[Singleton]
//public class FilterValidators : IFilterValidators
//{
//    // readonly IContainer _container;
//    readonly Func<IStreamProcessorStateRepository> _getStreamProcessorStates;
//    readonly Func<IFilterDefinitions> _getFilterDefinitions;
//    readonly IExecutionContextManager _executionContextManager;
//    readonly ICompareFilterDefinitions _definitionComparer;
//    readonly ILogger _logger;
//    readonly IDictionary<Type, Type> _filterDefinitionToValidatorMap = new Dictionary<Type, Type>();
//
//    /// <summary>
//    /// Initializes a new instance of the <see cref="FilterValidators"/> class.
//    /// </summary>
//    /// <param name="container">The <see cref="IContainer" />.</param>
//    /// <param name="getStreamProcessorStates">The <see cref="Func{T}" /> <see cref="IStreamProcessorStateRepository"/>.</param>
//    /// <param name="getFilterDefinitions">The <see cref="Func{T}" /> <see cref="IFilterDefinitions" />.</param>
//    /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
//    /// <param name="definitionComparer">The <see cref="ICompareFilterDefinitions" />.</param>
//    /// <param name="logger">The <see cref="ILogger" />.</param>
//    public FilterValidators(
//        //TODO: This used the ITypeFinder to find filter validators for filter definitions
//        // IContainer container,
//        Func<IStreamProcessorStateRepository> getStreamProcessorStates,
//        Func<IFilterDefinitions> getFilterDefinitions,
//        IExecutionContextManager executionContextManager,
//        ICompareFilterDefinitions definitionComparer,
//        ILogger logger)
//    {
//        // _container = container;
//        _getStreamProcessorStates = getStreamProcessorStates;
//        _getFilterDefinitions = getFilterDefinitions;
//        _executionContextManager = executionContextManager;
//        _definitionComparer = definitionComparer;
//        _logger = logger;
//        PopulateFilterValidatorMap();
//    }
//
//    /// <inheritdoc/>
//    public async Task<FilterValidationResult> Validate<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken)
//        where TDefinition : IFilterDefinition
//    {
//        var tryGetProcessorState = await _getStreamProcessorStates()
//            .TryGetFor(new StreamProcessorId(filter.Scope, filter.Definition.TargetStream.Value, filter.Definition.SourceStream), cancellationToken)
//            .ConfigureAwait(false);
//
//        if (!StreamProcessorHasProcessedEvents(tryGetProcessorState, out var validationResult, out var lastUnprocessedEvent))
//        {
//            return validationResult;
//        }
//
//        _logger.TryGetFilterDefinition(filter.Identifier, _executionContextManager.Current.Tenant);
//        var tryGetFilterDefinition = await _getFilterDefinitions().TryGetFromStream(filter.Scope, filter.Definition.TargetStream, cancellationToken).ConfigureAwait(false);
//
//        if (!FilterDefinitionHasBeenPersisted(tryGetFilterDefinition, out var persistedDefinition, out validationResult))
//        {
//            _logger.NoPersistedFilterDefinition(filter.Identifier, _executionContextManager.Current.Tenant);
//            return validationResult;
//        }
//
//        var definitionResult = _definitionComparer.DefinitionsAreEqual(persistedDefinition, filter.Definition);
//        if (!definitionResult.Success)
//        {
//            return definitionResult;
//        }
//
//        if (FilterDefinitionTypeHasChanged(persistedDefinition, filter.Definition))
//        {
//            return FilterValidationResult.Failed("Filter definition type has changed");
//        }
//
//        _logger.FindingFilterValidator(filter.Identifier);
//        if (!TryGetValidatorFor<TDefinition>(out var validator))
//        {
//            return FilterValidationResult.Failed($"No available filter validator for type {filter.Definition.GetType()}");
//        }
//
//        _logger.ValidatingFilter(filter.Identifier);
//        return await validator.Validate((TDefinition)persistedDefinition, filter, lastUnprocessedEvent, cancellationToken).ConfigureAwait(false);
//    }
//
//    bool StreamProcessorHasProcessedEvents(Try<IStreamProcessorState> tryGetState, out FilterValidationResult validationResult, out StreamPosition lastUnprocessedEvent)
//    {
//        if (!tryGetState.Success)
//        {
//            validationResult = tryGetState.Exception is StreamProcessorStateDoesNotExist
//                ? FilterValidationResult.Succeeded()
//                : FilterValidationResult.Failed(tryGetState.Exception.Message);
//            lastUnprocessedEvent = default;
//            return false;
//        }
//
//        lastUnprocessedEvent = tryGetState.Result.Position;
//
//        if (lastUnprocessedEvent == StreamPosition.Start)
//        {
//            validationResult = FilterValidationResult.Succeeded();
//            return false;
//        }
//
//        validationResult = default;
//        return true;
//    }
//
//    bool FilterDefinitionHasBeenPersisted(Try<IFilterDefinition> tryGetFilterDefinition, out IFilterDefinition persistedDefinition, out FilterValidationResult validationResult)
//    {
//        if (!tryGetFilterDefinition.Success)
//        {
//            validationResult = tryGetFilterDefinition.Exception is StreamDefinitionDoesNotExist
//                ? FilterValidationResult.Succeeded()
//                : FilterValidationResult.Failed(tryGetFilterDefinition.Exception.Message);
//            persistedDefinition = default;
//            return false;
//        }
//
//        persistedDefinition = tryGetFilterDefinition.Result;
//        validationResult = default;
//        return true;
//    }
//
//    bool FilterDefinitionTypeHasChanged<TDefinition>(IFilterDefinition persistedDefinition, TDefinition registeredDefinition)
//        => persistedDefinition.GetType() != registeredDefinition.GetType();
//
//    void PopulateFilterValidatorMap()
//    {
//        // TODO: Do this another way
//        // _typeFinder.FindMultiple<IFilterDefinition>().ForEach(filterDefinitionType =>
//        // {
//        //     if (TryGetValidatorTypeFor(filterDefinitionType, out var validatorType))
//        //     {
//        //         _logger.FoundValidatorForFilter(filterDefinitionType, validatorType);
//        //         _filterDefinitionToValidatorMap.TryAdd(filterDefinitionType, validatorType);
//        //     }
//        // });
//    }
//
//    bool TryGetValidatorTypeFor(Type filterDefinitionType, out Type validatorType)
//    {
//        // TODO: Do this another way
//        validatorType = null;
//        // var implementations = _typeFinder.FindMultiple(typeof(ICanValidateFilterFor<>).MakeGenericType(filterDefinitionType));
//        // if (implementations.Any())
//        // {
//        //     if (implementations.Count() > 1)
//        //     {
//        //         _logger.MultipleValidatorsForFilter(
//        //             filterDefinitionType,
//        //             implementations);
//        //     }
//        //
//        //     validatorType = implementations.First();
//        //     return true;
//        // }
//        //
//        // validatorType = null;
//        // return false;
//        return false;
//    }
//
//    bool TryGetValidatorFor<TDefinition>(out ICanValidateFilterFor<TDefinition> validator)
//        where TDefinition : IFilterDefinition
//    {
//        validator = null;
//        // TODO: Do this another way.
//        // if (_filterDefinitionToValidatorMap.TryGetValue(typeof(TDefinition), out var validatorType))
//        // {
//        //     validator = _container.Get(validatorType) as ICanValidateFilterFor<TDefinition>;
//        //     return true;
//        // }
//        //
//        // validator = null;
//        // return false;
//        return false;
//    }
//}

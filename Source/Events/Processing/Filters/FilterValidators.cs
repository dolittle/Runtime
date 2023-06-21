// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents an implementation of <see cref="IFilterValidators" />.
/// </summary>
[Singleton, PerTenant]
public class FilterValidators : IFilterValidators
{
    readonly TenantId _tenant;
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IFilterDefinitions _filterDefinitions;
    readonly ICompareFilterDefinitions _definitionComparer;
    readonly IServiceProvider _serviceProvider;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterValidators"/> class.
    /// </summary>
    /// <param name="tenant">The current tenant.</param>
    /// <param name="streamProcessorStates">The stream processor state repository to use to get the current state of the filter to validate.</param>
    /// <param name="filterDefinitions">The filter definitions to use to get the persisted definition of the filter to validate..</param>
    /// <param name="definitionComparer">The filter definition comparer to use to compare the filters.</param>
    /// <param name="serviceProvider">The service provider used to resolve the filter validator for a type of filter.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public FilterValidators(
        TenantId tenant,
        IStreamProcessorStates streamProcessorStates,
        IFilterDefinitions filterDefinitions,
        ICompareFilterDefinitions definitionComparer,
        IServiceProvider serviceProvider,
        ILogger logger)
    {
        _tenant = tenant;
        _streamProcessorStates = streamProcessorStates;
        _filterDefinitions = filterDefinitions;
        _definitionComparer = definitionComparer;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<FilterValidationResult> Validate<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken)
        where TDefinition : IFilterDefinition
    {
        if (!filter.Definition.CanBeValidated)
        {
            return FilterValidationResult.Succeeded();
        }
        
        var tryGetProcessorState = await _streamProcessorStates
            .TryGetFor(new StreamProcessorId(filter.Scope, filter.Definition.TargetStream.Value, filter.Definition.SourceStream), cancellationToken)
            .ConfigureAwait(false);

        if (!StreamProcessorHasProcessedEvents(tryGetProcessorState, out var validationResult, out var lastUnprocessedEvent))
        {
            return validationResult;
        }

        _logger.TryGetFilterDefinition(filter.Identifier, _tenant);
        var tryGetFilterDefinition = await _filterDefinitions.TryGetFromStream(filter.Scope, filter.Definition.TargetStream, cancellationToken).ConfigureAwait(false);

        if (!FilterDefinitionHasBeenPersisted(tryGetFilterDefinition, out var persistedDefinition, out validationResult))
        {
            _logger.NoPersistedFilterDefinition(filter.Identifier, _tenant);
            return validationResult;
        }

        var definitionResult = _definitionComparer.DefinitionsAreEqual(persistedDefinition, filter.Definition);
        if (!definitionResult.Success)
        {
            return definitionResult;
        }

        if (FilterDefinitionTypeHasChanged(persistedDefinition, filter.Definition))
        {
            return FilterValidationResult.Failed("Filter definition type has changed");
        }

        _logger.FindingFilterValidator(filter.Identifier);
        if (!TryGetValidatorFor<TDefinition>(out var validator))
        {
            return FilterValidationResult.Failed($"No available filter validator for type {filter.Definition.GetType()}");
        }

        _logger.ValidatingFilter(filter.Identifier);
        return await validator.Validate((TDefinition)persistedDefinition, filter, lastUnprocessedEvent, cancellationToken).ConfigureAwait(false);
    }

    bool TryGetValidatorFor<TDefinition>([NotNullWhen(true)] out ICanValidateFilterFor<TDefinition>? validator)
        where TDefinition : IFilterDefinition
    {
        validator = default;
        var validators = _serviceProvider.GetRequiredService<IList<ICanValidateFilterFor<TDefinition>>>();

        switch (validators.Count)
        {
            case < 1:
                return false;
            case > 1:
                _logger.MultipleValidatorsForFilter(typeof(TDefinition), validators.Select(_ => _.GetType()));
                return false;
            default:
                validator = validators[0];
                _logger.FoundValidatorForFilter(typeof(TDefinition), validator.GetType());
                return true;
        }
    }

    static bool StreamProcessorHasProcessedEvents(Try<IStreamProcessorState> tryGetState, [NotNullWhen(false)] out FilterValidationResult? validationResult, [NotNullWhen(true)] out ProcessingPosition? lastUnprocessedEvent)
    {
        if (!tryGetState.Success)
        {
            validationResult = tryGetState.Exception is StreamProcessorStateDoesNotExist
                ? FilterValidationResult.Succeeded()
                : FilterValidationResult.Failed(tryGetState.Exception.Message);
            lastUnprocessedEvent = default;
            return false;
        }

        lastUnprocessedEvent = tryGetState.Result.Position;

        if (lastUnprocessedEvent == ProcessingPosition.Initial)
        {
            validationResult = FilterValidationResult.Succeeded();
            return false;
        }

        validationResult = default;
        return true;
    }

    static bool FilterDefinitionHasBeenPersisted(Try<IFilterDefinition> tryGetFilterDefinition, out IFilterDefinition persistedDefinition, out FilterValidationResult validationResult)
    {
        if (!tryGetFilterDefinition.Success)
        {
            validationResult = tryGetFilterDefinition.Exception is StreamDefinitionDoesNotExist
                ? FilterValidationResult.Succeeded()
                : FilterValidationResult.Failed(tryGetFilterDefinition.Exception.Message);
            persistedDefinition = default;
            return false;
        }

        persistedDefinition = tryGetFilterDefinition.Result;
        validationResult = default;
        return true;
    }

    static bool FilterDefinitionTypeHasChanged<TDefinition>(IFilterDefinition persistedDefinition, TDefinition registeredDefinition)
        => persistedDefinition.GetType() != registeredDefinition.GetType();
}

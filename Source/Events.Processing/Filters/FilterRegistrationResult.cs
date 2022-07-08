// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents the result of registering a Filter.
/// </summary>
public class FilterRegistrationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterRegistrationResult"/> class.
    /// </summary>
    /// <param name="filterValidationResult">The failed <see cref="FilterValidationResult" />.</param>
    public FilterRegistrationResult(FilterValidationResult filterValidationResult)
    {
        FilterValidationResult = filterValidationResult;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterRegistrationResult"/> class.
    /// </summary>
    /// <param name="streamProcessor">The <see cref="StreamProcessors" />.</param>
    public FilterRegistrationResult(StreamProcessor streamProcessor)
    {
        StreamProcessor = streamProcessor;
        FilterValidationResult = FilterValidationResult.Succeeded();
        Success = streamProcessor != default;
    }

    /// <summary>
    /// Gets the <see cref="Processing.Filters.FilterValidationResult" />.
    /// </summary>
    public FilterValidationResult FilterValidationResult { get; }

    /// <summary>
    /// Gets the <see cref="Streams.StreamProcessor" />.
    /// </summary>
    public StreamProcessor StreamProcessor { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="StreamProcessor" /> was successfully registered.
    /// </summary>
    public bool Success { get; }
}
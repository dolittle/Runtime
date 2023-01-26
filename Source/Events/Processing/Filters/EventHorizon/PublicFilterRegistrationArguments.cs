// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon;

/// <summary>
/// Represents the runtime representation if of the public filter registarion request. 
/// </summary>
/// <param name="ExecutionContext">The execution context.</param>
/// <param name="Filter">The filter id.</param>
/// <returns></returns>
public record PublicFilterRegistrationArguments(ExecutionContext ExecutionContext, EventProcessorId Filter);
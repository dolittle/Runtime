// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Events.Processing.Filters.Unpartitioned;

/// <summary>
/// Represents the runtime representation of the unpartitioned filter registration arguments.
/// </summary>
/// <param name="ExecutionContext">The execution context.</param>
/// <param name="Filter">The filter id.</param>
/// <param name="Scope">The scope id.</param>
/// <returns></returns>
public record UnpartitionedFilterRegistrationArguments(ExecutionContext ExecutionContext, EventProcessorId Filter, ScopeId Scope);
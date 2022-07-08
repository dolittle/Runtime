// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Services.Contracts;
using ContractsExecutionContext = Dolittle.Execution.Contracts.ExecutionContext;

namespace Dolittle.Runtime.Protobuf;

/// <summary>
/// Extension methods for <see cref="ICreateExecutionContexts"/>.
/// </summary>
public static class ExecutionContextCreatorExtensions
{
    /// <summary>
    /// Tries to create an <see cref="ExecutionContext"/> from the <see cref="ContractsExecutionContext"/> present in a <see cref="CallRequestContext"/>.
    /// </summary>
    /// <param name="creator">This execution context creator.</param>
    /// <param name="requested">The execution context to use - typically from a Client request.</param>
    /// <returns>A <see cref="Try{TResult}"/> with a new <see cref="ExecutionContext"/> with the requested fields, if successful.</returns>
    public static Try<ExecutionContext> TryCreateUsing(this ICreateExecutionContexts creator, ContractsExecutionContext requested)
        => creator.TryCreateUsing(requested.ToExecutionContext());
}

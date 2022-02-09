// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Represents an implementation of <see cref="IAsyncPolicy"/>.
/// </summary>
public class AsyncPolicy : IAsyncPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPolicy"/> class.
    /// </summary>
    /// <param name="delegatedPolicy"><see cref="IAsyncPolicy"/> to delegate to.</param>
    public AsyncPolicy(IAsyncPolicy delegatedPolicy)
    {
        DelegatedPolicy = delegatedPolicy;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPolicy"/> class.
    /// </summary>
    /// <param name="underlyingPolicy">The underlying <see cref="Polly.IAsyncPolicy"/>.</param>
    public AsyncPolicy(Polly.IAsyncPolicy underlyingPolicy)
    {
        UnderlyingPolicy = underlyingPolicy;
    }

    /// <summary>
    /// Gets the underlying <see cref="Polly.ISyncPolicy">policy</see>.
    /// </summary>
    public Polly.IAsyncPolicy UnderlyingPolicy { get; }

    /// <summary>
    /// Gets the delegated <see cref="IPolicy"/>.
    /// </summary>
    public IAsyncPolicy DelegatedPolicy { get; }

    /// <inheritdoc/>
    public Task Execute(Func<Task> action) => Execute(_ => action(), CancellationToken.None);

    /// <inheritdoc/>
    public Task Execute(Func<CancellationToken, Task> action, CancellationToken cancellationToken) => Execute(action, false, cancellationToken);

    /// <inheritdoc/>
    public Task Execute(Func<CancellationToken, Task> action, bool continueOnCapturedContext, CancellationToken cancellationToken)
    {
        if (DelegatedPolicy != null)
        {
            return DelegatedPolicy.Execute(action, continueOnCapturedContext, cancellationToken);
        }
        else
        {
            return UnderlyingPolicy.ExecuteAsync(action, cancellationToken, continueOnCapturedContext);
        }
    }

    /// <inheritdoc/>
    public Task<TResult> Execute<TResult>(Func<Task<TResult>> action) => Execute(_ => action(), CancellationToken.None);

    /// <inheritdoc/>
    public Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken) => Execute(action, false, cancellationToken);

    /// <inheritdoc/>
    public Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> action, bool continueOnCapturedContext, CancellationToken cancellationToken)
    {
        if (DelegatedPolicy != null)
        {
            return DelegatedPolicy.Execute(action, continueOnCapturedContext, cancellationToken);
        }
        return UnderlyingPolicy.ExecuteAsync(action, cancellationToken, continueOnCapturedContext);
    }
}
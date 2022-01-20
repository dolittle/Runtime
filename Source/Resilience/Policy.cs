// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Represents an implementation of <see cref="IPolicy"/>.
/// </summary>
public class Policy : IPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Policy"/> class.
    /// </summary>
    /// <param name="delegatedPolicy"><see cref="IPolicy"/> to delegate to.</param>
    public Policy(IPolicy delegatedPolicy)
    {
        DelegatedPolicy = delegatedPolicy;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Policy"/> class.
    /// </summary>
    /// <param name="underlyingPolicy">The underlying <see cref="Polly.ISyncPolicy"/>.</param>
    public Policy(Polly.ISyncPolicy underlyingPolicy)
    {
        UnderlyingPolicy = underlyingPolicy;
    }

    /// <summary>
    /// Gets the underlying <see cref="Polly.ISyncPolicy">policy</see>.
    /// </summary>
    public Polly.ISyncPolicy UnderlyingPolicy { get; }

    /// <summary>
    /// Gets the delegated <see cref="IPolicy"/>.
    /// </summary>
    public IPolicy DelegatedPolicy { get; }

    /// <inheritdoc/>
    public void Execute(Action action)
    {
        if (DelegatedPolicy != null) DelegatedPolicy.Execute(action);
        else UnderlyingPolicy.Execute(action);
    }

    /// <inheritdoc/>
    public TResult Execute<TResult>(Func<TResult> action)
    {
        ThrowIfAsyncAction(typeof(TResult));
        if (DelegatedPolicy != null) return DelegatedPolicy.Execute(action);
        return UnderlyingPolicy.Execute(action);
    }

    void ThrowIfAsyncAction(Type type)
    {
        if (typeof(Task).IsAssignableFrom(type)) throw new SynchronousPolicyCannotReturnTask(type);
    }
}
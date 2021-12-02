// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Represents a <see cref="IAsyncPolicyFor{T}"/>.
/// </summary>
/// <typeparam name="T">The type the policy is for.</typeparam>
public class AsyncPolicyFor<T> : AsyncPolicy, IAsyncPolicyFor<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPolicyFor{T}"/> class.
    /// </summary>
    /// <param name="delegatedPolicy"><see cref="IPolicy"/> to delegate to.</param>
    public AsyncPolicyFor(IAsyncPolicy delegatedPolicy)
        : base(delegatedPolicy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPolicyFor{T}"/> class.
    /// </summary>
    /// <param name="underlyingPolicy">The underlying <see cref="Polly.IAsyncPolicy"/>.</param>
    public AsyncPolicyFor(Polly.IAsyncPolicy underlyingPolicy)
        : base(underlyingPolicy)
    {
    }
}
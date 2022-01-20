// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Represents a <see cref="IPolicyFor{T}"/>.
/// </summary>
/// <typeparam name="T">The type the policy is for.</typeparam>
public class PolicyFor<T> : Policy, IPolicyFor<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PolicyFor{T}"/> class.
    /// </summary>
    /// <param name="delegatedPolicy"><see cref="IPolicy"/> to delegate to.</param>
    public PolicyFor(IPolicy delegatedPolicy)
        : base(delegatedPolicy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolicyFor{T}"/> class.
    /// </summary>
    /// <param name="underlyingPolicy">The underlying <see cref="Polly.ISyncPolicy"/>.</param>
    public PolicyFor(Polly.ISyncPolicy underlyingPolicy)
        : base(underlyingPolicy)
    {
    }
}
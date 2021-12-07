// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Represents a <see cref="INamedAsyncPolicy"/>.
/// </summary>
public class NamedAsyncPolicy : AsyncPolicy, INamedAsyncPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedAsyncPolicy"/> class.
    /// </summary>
    /// <param name="name">Name of the policy.</param>
    /// <param name="delegatedPolicy"><see cref="IAsyncPolicy"/> to delegate to.</param>
    public NamedAsyncPolicy(string name, IAsyncPolicy delegatedPolicy)
        : base(delegatedPolicy)
    {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedAsyncPolicy"/> class.
    /// </summary>
    /// <param name="name">Name of the policy.</param>
    /// <param name="underlyingPolicy">The underlying <see cref="Polly.IAsyncPolicy"/>.</param>
    public NamedAsyncPolicy(string name, Polly.IAsyncPolicy underlyingPolicy)
        : base(underlyingPolicy)
    {
        Name = name;
    }

    /// <inheritdoc/>
    public string Name { get; }
}
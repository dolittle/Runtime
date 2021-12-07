// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Strategies;

/// <summary>
/// Represents an <see cref="IActivationStrategy"/> that gets activated through a callback.
/// </summary>
public class CallbackWithBindingContext : IActivationStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CallbackWithBindingContext"/> class.
    /// </summary>
    /// <param name="target">The callback target.</param>
    public CallbackWithBindingContext(Func<BindingContext, object> target) => Target = target;

    /// <summary>
    /// Gets the target.
    /// </summary>
    public Func<BindingContext, object> Target { get; }

    /// <inheritdoc/>
    public System.Type GetTargetType() => Target.GetType();
}
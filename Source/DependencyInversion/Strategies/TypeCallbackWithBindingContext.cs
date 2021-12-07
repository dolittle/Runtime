// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Strategies;

/// <summary>
/// Represents an <see cref="IActivationStrategy"/> that gets activated through a callback to a <see cref="System.Type"/>.
/// </summary>
public class TypeCallbackWithBindingContext : IActivationStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeCallbackWithBindingContext"/> class.
    /// </summary>
    /// <param name="target">The callback target.</param>
    public TypeCallbackWithBindingContext(Func<BindingContext, System.Type> target) => Target = target;

    /// <summary>
    /// Gets the target.
    /// </summary>
    public Func<BindingContext, System.Type> Target { get; }

    /// <inheritdoc/>
    public System.Type GetTargetType() => Target.Method.ReturnType;
}
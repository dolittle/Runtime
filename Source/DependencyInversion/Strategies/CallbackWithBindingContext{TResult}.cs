// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Strategies;

/// <summary>
/// Represents a generic <see cref="Callback"/>.
/// </summary>
/// <typeparam name="TResult">Type of result from the callback.</typeparam>
public class CallbackWithBindingContext<TResult> : CallbackWithBindingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CallbackWithBindingContext{TResult}"/> class.
    /// </summary>
    /// <param name="target">Target <see cref="Func{BindingContext, TResult}"/>.</param>
    public CallbackWithBindingContext(Func<BindingContext, TResult> target)
        : base((bindingContext) => target(bindingContext))
    {
    }
}
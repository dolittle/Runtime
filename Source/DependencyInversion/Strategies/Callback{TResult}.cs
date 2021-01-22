// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Strategies
{
    /// <summary>
    /// Represents a generic <see cref="Callback"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of result from the callback.</typeparam>
    public class Callback<TResult> : Callback
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Callback{TResult}"/> class.
        /// </summary>
        /// <param name="target">Target <see cref="Func{TResult}"/>.</param>
        public Callback(Func<TResult> target)
            : base(() => target())
        {
        }
    }
}
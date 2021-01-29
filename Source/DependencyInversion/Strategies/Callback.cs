// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Strategies
{
    /// <summary>
    /// Represents an <see cref="IActivationStrategy"/> that gets activated through a callback.
    /// </summary>
    public class Callback : IActivationStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Callback"/> class.
        /// </summary>
        /// <param name="target">The callback target.</param>
        public Callback(Func<object> target) => Target = target;

        /// <summary>
        /// Gets the target.
        /// </summary>
        public Func<object> Target { get; }

        /// <inheritdoc/>
        public System.Type GetTargetType() => Target.GetType();
    }
}
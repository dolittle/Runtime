// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Strategies
{
    /// <summary>
    /// Represents an <see cref="IActivationStrategy"/> that gets activated with a constant.
    /// </summary>
    public class Constant : IActivationStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="target">Target constant.</param>
        public Constant(object target) => Target = target;

        /// <summary>
        /// Gets the constant target.
        /// </summary>
        public object Target { get; }

        /// <inheritdoc/>
        public System.Type GetTargetType() => Target.GetType();
    }
}
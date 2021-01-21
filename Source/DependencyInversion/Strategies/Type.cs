// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Strategies
{
    /// <summary>
    /// Represents an <see cref="IActivationStrategy"/> that has a Type as its target.
    /// </summary>
    public class Type : IActivationStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Type"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> representing the target.</param>
        public Type(System.Type type)
        {
            Target = type;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> target.
        /// </summary>
        public System.Type Target { get; }

        /// <inheritdoc/>
        public System.Type GetTargetType()
        {
            return Target;
        }
    }
}
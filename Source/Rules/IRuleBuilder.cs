// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Defines the basis for a rule builder.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="IRule"/> the builder is for.</typeparam>
    public interface IRuleBuilder<T>
        where T : IRule
    {
        /// <summary>
        /// Gets the rules from the builder.
        /// </summary>
        IEnumerable<T> Rules { get; }

        /// <summary>
        /// Add a rule to the builder.
        /// </summary>
        /// <param name="rule">A rule that implements <see cref="IRule"/> to add.</param>
        void AddRule(T rule);
    }
}

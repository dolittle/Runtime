// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store.State
{
    /// <summary>
    /// Represents a state from a embedding.
    /// </summary>
    /// <param name="Value">The state.</param>
    /// <typeparam name="string">The type of the concept.</typeparam>
    public record EmbeddingState(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly convers the string to a <see cref="EmbeddingState" />.
        /// </summary>
        /// <param name="state">The <see cref="EmbeddingState" />.</param>
        public static implicit operator EmbeddingState(string state) => new(state);
    }
}

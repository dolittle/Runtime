// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Exception that gets thrown when attempting to convert a <see cref="EmbeddingCurrentStateType"/> that is not known.
    /// </summary>
    public class UnknownEmbeddingCurrentStateType : Exception
    {
        /// <summary>
        /// Initializes an instance of the <see cref="UnknownEmbeddingCurrentStateType"/> class.
        /// </summary>
        /// <param name="type">The current state type that is not known.</param>
        public UnknownEmbeddingCurrentStateType(EmbeddingCurrentStateType type)
            : base($"{nameof(EmbeddingCurrentStateType)} {type} is not known")
        {
        }
    }
}
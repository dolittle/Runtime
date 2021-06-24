// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Represents the different types of a <see cref="EmbeddingCurrentState" />.
    /// </summary>
    public enum EmbeddingCurrentStateType
    {
        CreatedFromInitialState = 0,
        Persisted,
        Deleted
    }
}

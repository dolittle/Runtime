// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines the protocol for embeddings.
    /// </summary>
    public interface IEmbeddingsProtocol : IReverseCallServiceProtocol<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse, EmbeddingRegistrationArguments>
    {
    }
}

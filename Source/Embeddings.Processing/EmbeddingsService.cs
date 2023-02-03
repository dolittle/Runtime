// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Services;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Protobuf;
using System.Threading;
using Dolittle.Runtime.Rudimentary;
using System.Linq;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Services.Hosting;
using static Dolittle.Runtime.Embeddings.Contracts.Embeddings;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents the implementation of <see cref="EmbeddingsBase"/>.
/// </summary>
[PrivateService]
public class EmbeddingsService : EmbeddingsBase
{
    readonly ILogger<EmbeddingsService> _logger;

    /// <summary>
    /// Initializes an instance of the <see cref="EmbeddingsService" /> class.
    /// </summary>
    /// <param name="hostApplicationLifetime"></param>
    /// <param name="executionContextCreator"></param>
    /// <param name="reverseCallServices"></param>
    /// <param name="protocol"></param>
    /// <param name="getEmbeddingProcessorFactoryFor"></param>
    /// <param name="embeddingProcessors"></param>
    /// <param name="embeddingRequestFactory"></param>
    /// <param name="embeddingDefinitionComparer"></param>
    /// <param name="embeddingDefinitionPersister"></param>
    /// <param name="logger"></param>
    public EmbeddingsService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EmbeddingsService>();
    }

    public override Task Connect(IAsyncStreamReader<EmbeddingClientToRuntimeMessage> requestStream,
        IServerStreamWriter<EmbeddingRuntimeToClientMessage> responseStream, ServerCallContext context)
    {
        _logger.LogError("Embeddings was attempted used!");
        throw new NotImplementedException("Embeddings have been removed. Use aggregates instead!");
    }

    public override Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
    {
        _logger.LogError("Embeddings was attempted used!");
        throw new NotImplementedException("Embeddings have been removed. Use aggregates instead!");
    }

    public override Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        _logger.LogError("Embeddings was attempted used!");
        throw new NotImplementedException("Embeddings have been removed. Use aggregates instead!");
    }
}

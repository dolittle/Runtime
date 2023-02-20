// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Dolittle.Services.Contracts;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Status = Grpc.Core.Status;
using Version = Dolittle.Runtime.Domain.Platform.Version;


namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.given;

delegate bool TryGetEmbeddingProcessorForReturns(TenantId tenant, EmbeddingId embedding, out IEmbeddingProcessor processor);
public class all_dependencies
{
    protected static Mock<IHostApplicationLifetime> application_lifetime;
    protected static Mock<IInitiateReverseCallServices> reverse_call_services;
    protected static Mock<Func<TenantId, IEmbeddingProcessorFactory>> get_embedding_processor_factory;
    protected static Mock<IEmbeddingProcessors> embedding_processors;
    protected static Mock<ICreateExecutionContexts> execution_context_creator;
    protected static Mock<IEmbeddingProcessor> embedding_processor;
    protected static IEmbeddingRequestFactory embedding_request_factory;
    protected static Mock<ICompareEmbeddingDefinitionsForAllTenants> embedding_definition_comparer;
    protected static Mock<IPersistEmbeddingDefinitionForAllTenants> embedding_definition_persister;
    protected static ILoggerFactory logger_factory;
    protected static EmbeddingsService embedding_service;
    protected static IEmbeddingsProtocol protocol;
    protected static Mock<IAsyncStreamReader<EmbeddingClientToRuntimeMessage>> runtime_stream;
    protected static Mock<IServerStreamWriter<EmbeddingRuntimeToClientMessage>> client_stream;
    protected static Mock<IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>> dispatcher;
    protected static ServerCallContext call_context;
    protected static ExecutionContext execution_context;
    protected static EmbeddingId embedding_id;
    protected static IEnumerable<Artifact> events;
    protected static ProjectionState initial_state;
    protected static EmbeddingDefinition embedding_definition;
    protected static CallRequestContext call_request_context;

    private Establish context = () =>
    {
        embedding_id = "814b9978-2a4c-44cb-a765-3a7caadc1ee4";
        execution_context = new ExecutionContext(
            "f3b5157e-6f27-4a5e-92ea-f2768e68d9ed",
            "4875bdd1-4866-4d2f-a2d0-967fe7a4872e",
            Version.NotSet,
            "Environment",
            "14d2ce96-ada9-4446-8abb-6e12a49afd39",
            ActivitySpanId.CreateRandom(),
            Execution.Claims.Empty,
            CultureInfo.InvariantCulture);
        execution_context_creator = new Mock<ICreateExecutionContexts>();
        execution_context_creator
            .Setup(_ => _.TryCreateUsing(Moq.It.IsAny<ExecutionContext>()))
            .Returns<ExecutionContext>(_ => _);
        
        call_request_context = new CallRequestContext
        {
            ExecutionContext = execution_context.ToProtobuf()
        };
        events = new[]
        {
            new Artifact("4f8cbf53-ce87-48ee-b24a-a1c0e654282a", 1)
        };
        initial_state = "some initial state";
        embedding_definition = new EmbeddingDefinition(embedding_id, events, initial_state);
        application_lifetime = new Mock<IHostApplicationLifetime>();
        application_lifetime
            .SetupGet(_ => _.ApplicationStopping)
            .Returns(CancellationToken.None);
        reverse_call_services = new Mock<IInitiateReverseCallServices>();
        logger_factory = NullLoggerFactory.Instance;
        get_embedding_processor_factory = new Mock<Func<TenantId, IEmbeddingProcessorFactory>>();
        embedding_processors = new Mock<IEmbeddingProcessors>();
        embedding_processor = new Mock<IEmbeddingProcessor>();
        protocol = new EmbeddingsProtocol();
        embedding_request_factory = new EmbeddingRequestFactory();
        embedding_definition_comparer = new Mock<ICompareEmbeddingDefinitionsForAllTenants>();
        embedding_definition_comparer
            .Setup(_ => _.DiffersFromPersisted(Moq.It.IsAny<EmbeddingDefinition>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IDictionary<TenantId, EmbeddingDefinitionComparisonResult>>(new Dictionary<TenantId, EmbeddingDefinitionComparisonResult>()));
        embedding_definition_persister = new Mock<IPersistEmbeddingDefinitionForAllTenants>();
        embedding_definition_persister
            .Setup(_ => _.TryPersist(Moq.It.IsAny<EmbeddingDefinition>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try.Succeeded()));
        embedding_service = new EmbeddingsService(
            application_lifetime.Object,
            execution_context_creator.Object,
            reverse_call_services.Object, protocol,
            get_embedding_processor_factory.Object,
            embedding_processors.Object,
            embedding_definition_comparer.Object,
            embedding_definition_persister.Object,
            embedding_request_factory,
            NullLoggerFactory.Instance);
        dispatcher = new Mock<IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>>();
        runtime_stream = new Mock<IAsyncStreamReader<EmbeddingClientToRuntimeMessage>>();
        client_stream = new Mock<IServerStreamWriter<EmbeddingRuntimeToClientMessage>>();
        
        call_context = CallContext.Create();
    };
    

}

public class CallContext : ServerCallContext
{ 
    readonly Metadata _requestHeaders;
    readonly CancellationToken _cancellationToken;
    readonly Metadata _responseTrailers;
    readonly AuthContext _authContext;
    readonly Dictionary<object, object> _userState;
    WriteOptions? _writeOptions;
    
    public Metadata? ResponseHeaders { get; private set; }

    private CallContext(Metadata requestHeaders, CancellationToken cancellationToken)
    {
        _requestHeaders = requestHeaders;
        _cancellationToken = cancellationToken;
        _responseTrailers = new Metadata();
        _authContext = new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>());
        _userState = new Dictionary<object, object>();
    }

    protected override string MethodCore => "MethodName";
    protected override string HostCore => "HostName";
    protected override string PeerCore => "PeerName";
    protected override DateTime DeadlineCore { get; }
    protected override Metadata RequestHeadersCore => _requestHeaders;
    protected override CancellationToken CancellationTokenCore => _cancellationToken;
    protected override Metadata ResponseTrailersCore => _responseTrailers;
    protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get => _writeOptions; set { _writeOptions = value; } }
    protected override AuthContext AuthContextCore => _authContext;

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options)
    {
        throw new NotImplementedException();
    }

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
    {
        if (ResponseHeaders != null)
        {
            throw new InvalidOperationException("Response headers have already been written.");
        }

        ResponseHeaders = responseHeaders;
        return Task.CompletedTask;
    }

    protected override IDictionary<object, object> UserStateCore => _userState;

    public static CallContext Create(Metadata? requestHeaders = null, CancellationToken cancellationToken = default)
    {
        return new CallContext(requestHeaders ?? new Metadata(), cancellationToken);
    }
}
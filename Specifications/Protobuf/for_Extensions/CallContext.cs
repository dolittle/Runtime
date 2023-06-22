// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.Runtime.Protobuf.for_Extensions;


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
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher
{
    public class CallContext : ServerCallContext
    {
        public CallContext()
        {
            RequestHeadersCore = new Metadata();
        }

        protected override string MethodCore => "SpecMethod";

        protected override string HostCore => throw new NotImplementedException();

        protected override string PeerCore => throw new NotImplementedException();

        protected override DateTime DeadlineCore => throw new NotImplementedException();

        protected override Metadata RequestHeadersCore { get; }

        protected override CancellationToken CancellationTokenCore => CancellationToken.None;

        protected override Metadata ResponseTrailersCore => throw new NotImplementedException();

        protected override Status StatusCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override WriteOptions WriteOptionsCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override AuthContext AuthContextCore => throw new NotImplementedException();

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
        {
            throw new NotImplementedException();
        }
    }
}
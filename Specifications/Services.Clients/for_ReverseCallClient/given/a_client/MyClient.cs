// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Grpc.Core;
using Moq;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client
{
    public class MyClient : ClientBase<MyClient>
    {
        readonly Mock<IAsyncStreamReader<MyServerMessage>> _server_stream;
        readonly Mock<IClientStreamWriter<MyClientMessage>> _client_stream;

        public MyClient(Mock<IAsyncStreamReader<MyServerMessage>> server_stream, Mock<IClientStreamWriter<MyClientMessage>> client_stream)
        {
            _server_stream = server_stream;
            _client_stream = client_stream;
        }

        protected override MyClient NewInstance(ClientBaseConfiguration configuration)
            => new(new Mock<IAsyncStreamReader<MyServerMessage>>(), new Mock<IClientStreamWriter<MyClientMessage>>());

        public AsyncDuplexStreamingCall<MyClientMessage, MyServerMessage> Method(CallOptions options)
        {
            return new AsyncDuplexStreamingCall<MyClientMessage, MyServerMessage>(
                _client_stream.Object,
                _server_stream.Object,
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }
    }
}

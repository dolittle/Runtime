// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher
{
    public class MyServerMessage : IMessage
    {
        public MyConnectResponse ConnectResponse { get; set; }

        public MyRequest Request { get; set; }

        public Ping Ping { get; set; }

        public MessageDescriptor Descriptor => throw new System.NotImplementedException();

        public int CalculateSize()
        {
            return 0;
        }

        public void MergeFrom(CodedInputStream input)
        {
        }

        public void WriteTo(CodedOutputStream output)
        {
        }
    }
}

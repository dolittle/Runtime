// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Grpc.Core;

namespace Dolittle.Runtime.Services.ReverseCalls.given;

public class a_message : IMessage
{
    public MessageDescriptor Descriptor => throw new NotImplementedException();

    public int CalculateSize()
        => 1;

    public void MergeFrom(CodedInputStream input)
    {
    }

    public void WriteTo(CodedOutputStream output)
    {
    }
}
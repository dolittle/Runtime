// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Dolittle.Runtime.Protobuf.for_Extensions;

public class MyMessage : IMessage<MyMessage>
{
    public MessageDescriptor Descriptor => throw new NotImplementedException();

    public int CalculateSize()
    {
        return 0;
    }

    public MyMessage Clone()
    {
        throw new NotImplementedException();
    }

    public bool Equals([AllowNull] MyMessage other)
    {
        throw new NotImplementedException();
    }

    public void MergeFrom(MyMessage message)
    {
        throw new NotImplementedException();
    }

    public void MergeFrom(CodedInputStream input)
    {
        throw new NotImplementedException();
    }

    public void WriteTo(CodedOutputStream output)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
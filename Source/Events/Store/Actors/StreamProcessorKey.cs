// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.Actors;

public partial class StreamProcessorKey
{
    public IStreamProcessorId FromProtobuf()
    {
        switch (idCase_)
        {
            case IdOneofCase.StreamProcessorId:
                return Processing.Streams.StreamProcessorId.FromProtobuf(StreamProcessorId);
            case IdOneofCase.SubscriptionId:
                return Runtime.EventHorizon.Consumer.SubscriptionId.FromProtobuf(SubscriptionId);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

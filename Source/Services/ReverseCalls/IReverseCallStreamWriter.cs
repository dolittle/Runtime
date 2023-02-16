// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.ReverseCalls;

public interface IReverseCallStreamWriter<TServerMessage> : IAsyncStreamWriter<TServerMessage>, IDisposable
    where TServerMessage : IMessage, new()
{
    /// <summary>
    /// Writes a ping message synchronously if another write operation is not in progress.
    /// </summary>
    void MaybeWritePing();
}

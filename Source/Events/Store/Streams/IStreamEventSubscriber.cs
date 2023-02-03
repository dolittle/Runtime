// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;

namespace Dolittle.Runtime.Events.Store.Streams;


public interface IStreamEventSubscriber
{
    public IAsyncEnumerable<StreamEvent> SubscribePublic(ProcessingPosition position, CancellationToken cancellationToken);
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.EventHandlers;

namespace Dolittle.Runtime.Events.Store;

public interface IFetchStartPosition
{
    public ValueTask<EventLogSequenceNumber> GetInitialProcessorSequence(ScopeId scopeId, StartFrom startFrom, CancellationToken cancellationToken);
}

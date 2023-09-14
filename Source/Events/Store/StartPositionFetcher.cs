// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Processing.EventHandlers;

namespace Dolittle.Runtime.Events.Store;

[PerTenant]
public class StartPositionFetcher: IFetchStartPosition
{
    readonly IFetchCommittedEvents _fetcher;

    public StartPositionFetcher(IFetchCommittedEvents fetcher)
    {
        _fetcher = fetcher;
    }

    public async ValueTask<EventLogSequenceNumber> GetInitialProcessorSequence(ScopeId scopeId, StartFrom startFrom, CancellationToken cancellationToken)
    {
        if (startFrom.StartFromLatest)
        {
            return await _fetcher.FetchNextSequenceNumber(scopeId, cancellationToken);
        }
        
        if (startFrom.SpecificTimestamp.HasValue)
        {
            return await _fetcher.FetchNextSequenceNumberAfter(scopeId, startFrom.SpecificTimestamp.Value, cancellationToken);
        }
        
        // Default, start from the beginning
        return EventLogSequenceNumber.Initial;
    }
    
}

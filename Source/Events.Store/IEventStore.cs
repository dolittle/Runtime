// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines the functionality required for an EventStore implementation.
/// </summary>
public interface IEventStore : ICommitEvents, IFetchCommittedEvents
{
}
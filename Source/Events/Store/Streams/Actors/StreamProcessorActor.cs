// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Legacy;



namespace Dolittle.Runtime.Events.Store.Streams.Actors;

public class StreamProcessorActor
{
    readonly IStreamEventSubscriber _subscriber;
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IMapStreamPositionToEventLogPosition _eventLogPositionEnricher;


}

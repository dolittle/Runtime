// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers;

/// <summary>
/// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Event Handlers.
/// </summary>
public static class EventHandlersFailures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventHandlerNotRegistered' failure type.
    /// </summary>
    public static FailureId EventHandlerNotRegistered => FailureId.Create("000669b7-2ad3-4c4c-a245-55ba33c48a4d");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventHandlerNotRegisteredForTenant' failure type.
    /// </summary>
    public static FailureId EventHandlerNotRegisteredForTenant => FailureId.Create("7bc340c0-c0e1-4ef6-9c44-20304bfc2ef2");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'AlreadySettingNewStreamProcessorPosition' failure type.
    /// </summary>
    public static FailureId AlreadySettingEventHandlerPosition => FailureId.Create("f5d9c2db-c6e3-4d12-848c-c2981c4ffb85");
        
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'CannotReprocessEventsFromPositionHigherThanCurrentPosition' failure type.
    /// </summary>
    public static FailureId CannotReprocessEventsFromPositionHigherThanCurrentPosition => FailureId.Create("d3a0bafc-4746-41ed-b4a4-e67b50559703");
}
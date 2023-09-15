// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

public sealed class StartFrom
{
    public static readonly StartFrom Earliest = new();
    public static readonly StartFrom Latest = new();
    public static readonly StartFrom Default = Earliest;

    public DateTimeOffset? SpecificTimestamp { get; init; }

    public bool StartFromEarliest => this == Earliest;
    public bool StartFromLatest => this == Latest;
}

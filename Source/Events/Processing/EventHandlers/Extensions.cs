// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

static class Extensions
{
    public static StartFrom FromProtobuf(this Contracts.StartFrom? startFrom)
    {
        if (startFrom is null)
        {
            return StartFrom.Default;
        }

        switch (startFrom.SelectedCase)
        {
            case Contracts.StartFrom.SelectedOneofCase.None:
                return StartFrom.Default;
            case Contracts.StartFrom.SelectedOneofCase.Position:
                return startFrom.Position switch
                {
                    Contracts.StartFrom.Types.Position.Start => StartFrom.Earliest,
                    Contracts.StartFrom.Types.Position.Latest => StartFrom.Latest,
                    _ => StartFrom.Default
                };
            case Contracts.StartFrom.SelectedOneofCase.Timestamp:
                return new StartFrom
                {
                    SpecificTimestamp = startFrom.Timestamp.ToDateTimeOffset()
                };
            default:
                return StartFrom.Default;
        }
    }
}

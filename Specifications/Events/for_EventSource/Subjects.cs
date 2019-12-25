// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Events.Specs.for_EventSource
{
    public static class Subjects
    {
        public const string reapplying_events = "ReApplying events";
        public const string applying_events = "Applying events";
        public const string committing_events = "Committing events";
        public const string creating_event_source = "Creating event source";
        public const string rolling_back = "Rolling back";
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Specs.given
{
    public abstract class Events
    {
        public static readonly SimpleEvent event_one = new SimpleEvent { Content = "One" };
        public static readonly SimpleEvent event_two = new SimpleEvent { Content = "Two" };
        public static readonly AnotherEvent event_three = new AnotherEvent { Content = "Two" };
    }
}
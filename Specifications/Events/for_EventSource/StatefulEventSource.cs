// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class StatefulEventSource : EventSource
    {
        public bool CommitCalled = false;
        public bool RollbackCalled = false;

        public StatefulEventSource(Guid id)
            : base(id)
        {
        }

        public string Value { get; set; }

        public bool EventApplied { get; private set; }

        public override void Commit()
        {
            CommitCalled = true;
            base.Commit();
        }

        public override void Rollback()
        {
            RollbackCalled = true;
            base.Rollback();
        }

        void On(SimpleEvent simpleEvent)
        {
            EventApplied = true;
            Value = simpleEvent.Content;
        }
    }
}
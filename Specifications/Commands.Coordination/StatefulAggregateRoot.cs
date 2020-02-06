// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Commands.Coordination.Specs
{
    public class StatefulAggregateRoot : AggregateRoot
    {
        public bool CommitCalled = false;
        public bool RollbackCalled = false;

        public string Value { get; set; }

        public bool EventApplied { get; private set; }

        public StatefulAggregateRoot(Guid id)
            : base(id)
        {
        }

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
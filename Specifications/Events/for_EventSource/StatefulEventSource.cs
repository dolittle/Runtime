using System;

namespace Dolittle.Runtime.Events.Specs.for_EventSource
{
    public class StatefulEventSource : EventSource
    {
        public string Value { get; set; }
        public bool EventApplied { get; private set; }

        public StatefulEventSource(Guid id) : base(id)
        {
        }

        void On(SimpleEvent simpleEvent)
        {
            EventApplied = true;
            Value = simpleEvent.Content;
        }


        public bool CommitCalled = false;
        public override void Commit()
        {
            CommitCalled = true;
            base.Commit();
        }

        public bool RollbackCalled = false;
        public override void Rollback()
        {
            RollbackCalled = true;
            base.Rollback();
        }
    }
}
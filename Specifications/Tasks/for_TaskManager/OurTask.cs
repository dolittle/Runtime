// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Tasks.Specs.for_TaskManager
{
    public class OurTask : Task
    {
        public override TaskOperation[] Operations
        {
            get
            {
                return new TaskOperation[]
                {
                    FirstOperation,
                    SecondOperation
                };
            }
        }

        public bool FirstOperationCalled;

        public void FirstOperation(Task task, int operationIndex)
        {
            FirstOperationCalled = true;
        }

        public bool SecondOperationCalled;

        public void SecondOperation(Task task, int operationIndex)
        {
            SecondOperationCalled = true;
        }

        public bool BeginCalled;

        public override void Begin()
        {
            BeginCalled = true;
            base.Begin();
        }

        public bool EndCalled = false;

        public override void End()
        {
            EndCalled = true;
            base.End();
        }
    }
}

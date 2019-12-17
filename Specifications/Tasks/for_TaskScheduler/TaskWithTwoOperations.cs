// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Tasks.Specs.for_TaskScheduler
{
    public class TaskWithTwoOperations : Task
    {
        bool _runAsynchronously;

        public TaskWithTwoOperations(bool runAsynchronously)
        {
            _runAsynchronously = runAsynchronously;
        }

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

        public bool FirstOperationCalled = false;
        public int FirstOperationIndex = -1;
        public Action FirstOperationCallback;

        void FirstOperation(Task task, int operationIndex)
        {
            FirstOperationCalled = true;
            FirstOperationIndex = operationIndex;
            FirstOperationCallback?.Invoke();
        }

        public bool SecondOperationCalled = false;
        public int SecondOperationIndex = -1;
        public Action SecondOperationCallback;

        void SecondOperation(Task task, int operationIndex)
        {
            SecondOperationCalled = true;
            SecondOperationIndex = operationIndex;
            SecondOperationCallback?.Invoke();
        }

        public override bool CanRunOperationsAsynchronously { get { return _runAsynchronously; } }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Exception that gets thrown when a synchronous policy is executing an asynchronous action.
    /// </summary>
    public class SynchronousPolicyCannotReturnTask : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousPolicyCannotReturnTask"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Task" /> <see cref="Type" />.</param>
        public SynchronousPolicyCannotReturnTask(Type type)
            : base($"Cannot execute synchronous policy on action that returns a Task. '{type.FullName}' is a Task.")
        {
        }
    }
}
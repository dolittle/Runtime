// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Exception that gets thrown when a target is not alive in a weak reference.
    /// </summary>
    public class CannotInvokeMethodBecauseTargetIsNotAlive : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotInvokeMethodBecauseTargetIsNotAlive"/> class.
        /// </summary>
        /// <param name="method"><see cref="MethodInfo"/> for the method that can't be invoked.</param>
        public CannotInvokeMethodBecauseTargetIsNotAlive(MethodInfo method)
            : base($"Method '{method}' can't be invoked, since target has been collected by the garbage collector")
        {
        }
    }
}

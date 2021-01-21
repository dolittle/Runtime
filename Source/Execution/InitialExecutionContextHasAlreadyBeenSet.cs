// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Exception that gets thrown when the execution context has initially been set already.
    /// </summary>
    public class InitialExecutionContextHasAlreadyBeenSet : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialExecutionContextHasAlreadyBeenSet"/> class.
        /// </summary>
        public InitialExecutionContextHasAlreadyBeenSet()
            : base("Initial execution context has already been set - it can't set twice in the same process")
        {
        }
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when a commit is duplicate.
    /// </summary>
    public class CommitIsADuplicate : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitIsADuplicate"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public CommitIsADuplicate(string message)
            : base(message)
        {
        }
    }
}
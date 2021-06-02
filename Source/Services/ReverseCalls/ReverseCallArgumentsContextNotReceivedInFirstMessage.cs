// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Services.Contracts;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Exception that gets thrown when the first message of a reverse call connection does not contain a <see cref="ReverseCallArgumentsContext"/>.
    /// </summary>
    public class ReverseCallArgumentsContextNotReceivedInFirstMessage : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallArgumentsContextNotReceivedInFirstMessage"/> class.
        /// </summary>
        /// <param name="reason">The reason for the missing reverse call arguments context.</param>
        public ReverseCallArgumentsContextNotReceivedInFirstMessage(string reason)
            : base($"The first message received for the reverse call connection did not contain a {nameof(ReverseCallArgumentsContext)} because {reason}")
        {
        }
    }
}
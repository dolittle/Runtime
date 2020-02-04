// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="StreamProcessorKey" /> is already registered.
    /// </summary>
    public class StreamProcessorKeyAlreadyRegistered : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorKeyAlreadyRegistered"/> class.
        /// </summary>
        /// <param name="key">The <see cref="StreamProcessorKey" />.</param>
        public StreamProcessorKeyAlreadyRegistered(StreamProcessorKey key)
            : base($"The stream processor key '{key}' is already registered")
        {
        }
    }
}
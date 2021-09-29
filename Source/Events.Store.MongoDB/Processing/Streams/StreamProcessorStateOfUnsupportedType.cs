// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when trying to use an unsuported <see cref="IStreamProcessorState"/> and <see cref="IStreamProcessorId"/> and .
    /// </summary>
    public class StreamProcessorStateOfUnsupportedType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStateOfUnsupportedType"/> class.
        /// </summary>
        /// <param name="id">The failed id.</param>
        /// <param name="state">The failed state class.</param>
        public StreamProcessorStateOfUnsupportedType(IStreamProcessorId id, IStreamProcessorState state)
            : base($"Unsupported StreamProcessorState: {state} with IStreamProcessorId: {id}")
        {
        }
    }
}

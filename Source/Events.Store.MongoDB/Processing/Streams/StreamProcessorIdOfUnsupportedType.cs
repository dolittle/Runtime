// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when trying to use an unsupported <see cref="IStreamProcessorId"/>.
    /// </summary>
    public class StreamProcessorIdOfUnsupportedType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorIdOfUnsupportedType"/> class.
        /// </summary>
        /// <param name="id">The unsupported id.</param>
        public StreamProcessorIdOfUnsupportedType(IStreamProcessorId id)
            : base($"Unsupported IStreamProcessorId: {id}")
        {
        }
    }
}

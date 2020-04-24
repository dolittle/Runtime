// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when an illegal filter transformation is taking place.
    /// </summary>
    public class IllegalFilterTransformation : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalFilterTransformation"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        public IllegalFilterTransformation(ScopeId scope, StreamId targetStream, StreamId sourceStream)
            : base($"Cannot register filter with target stream '{targetStream}' and source stream '{sourceStream}' in scope '{scope}' because it produces a different stream compared to the previously registered filter. Change the target Stream Id of the new filter to create a new stream.")
        {
        }
    }
}
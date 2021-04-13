// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines the basis of a unique identifier of a Stream Processor.
    /// </summary>
    public interface IStreamProcessorId
    {
        /// <summary>
        /// Gets the <see cref="ScopeId" />.
        /// </summary>
        ScopeId ScopeId { get; }
    }
}
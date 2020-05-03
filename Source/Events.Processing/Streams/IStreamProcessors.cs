// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a hub for <see cref="AbstractScopedStreamProcessor" />.
    /// </summary>
    public interface IStreamProcessors
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" /> of the stream the <see cref="AbstractScopedStreamProcessor" /> is processing.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <param name="streamProcessor">The registered <see cref="StreamProcessor" />.</param>
        /// <returns>A value indicating whether a new <see cref="StreamProcessor" /> was registered.</returns>
        bool TryRegister(IStreamDefinition streamDefinition, IEventProcessor eventProcessor, CancellationToken cancellationToken, out StreamProcessor streamProcessor);
    }
}
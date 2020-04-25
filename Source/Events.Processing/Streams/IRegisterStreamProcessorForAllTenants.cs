// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a system that can register a <see cref="StreamProcessor" /> for all tenants.
    /// </summary>
    public interface IRegisterStreamProcessorForAllTenants
    {
        /// <summary>
        /// Register a <see cref="StreamProcessor" /> with the given <see cref="IEventProcessor" /> on the given source <see cref="StreamId" />.
        /// </summary>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="streamDefinitionGetter">A <see cref="Task" /> that, when resolved, returns the <see cref="StreamDefinition" />.</param>
        /// <param name="streamProcessorRegistrations">The <see cref="StreamProcessorRegistrations" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Register(IEventProcessor eventProcessor, Task<StreamDefinition> streamDefinitionGetter, StreamProcessorRegistrations streamProcessorRegistrations, CancellationToken cancellationToken);
    }
}

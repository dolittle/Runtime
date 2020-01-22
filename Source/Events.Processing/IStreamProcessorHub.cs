// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" />.
    /// </summary>
    public interface IStreamProcessorHub
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="eventProcessor">The <see cref="IEventProcessorNew" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="tenantId">The <see cref="TenantId" />.</param>
        /// <returns>The registered <see cref="StreamProcessor" />.</returns>
        StreamProcessor Register(IEventProcessorNew eventProcessor, StreamId sourceStreamId, TenantId tenantId);
    }
}
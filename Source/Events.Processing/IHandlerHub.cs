// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" /> interacting with a <see cref="HandlerProcessor" />.
    /// </summary>
    public interface IHandlerHub
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="handlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="sourceStream">The <see cref="StreamId" />.</param>
        /// <param name="tenantId">The <see cref="TenantId" />.</param>
        void Register(EventHandlerId handlerId, StreamId sourceStream, TenantId tenantId);
    }
}
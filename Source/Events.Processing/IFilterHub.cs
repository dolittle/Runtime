// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" /> interacting with a <see cref="FilterProcessor" />.
    /// </summary>
    public interface IFilterHub
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="targetStreamId">The <see cref="StreamId" />.</param>
        /// <param name="tenantId">The <see cref="TenantId" />.</param>
        void Register(FilterId filterId, StreamId targetStreamId, TenantId tenantId);
    }
}
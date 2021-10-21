// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when a Stream Processor is not registered for a specific tenant.
    /// </summary>
    public class StreamProcessorNotRegisteredForTenant : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorNotRegisteredForTenant"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        /// <param name="tenant">The <see cref="TenantId"/>.</param>
        public StreamProcessorNotRegisteredForTenant(IStreamProcessorId streamProcessorId, TenantId tenant)
            : base($"Stream Processor: '{streamProcessorId}' is not registered for tenant '{tenant}'")
        {
        }
    }
}
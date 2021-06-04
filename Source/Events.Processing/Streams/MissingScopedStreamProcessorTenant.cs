// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that is thrown when a <see cref="AbstractScopedStreamProcessor"/> is missing for a specific <see cref="TenantId"/>.
    /// </summary>
    public class MissingScopedStreamProcessorTenant : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MissingScopedStreamProcessorTenant"/>;
        /// </summary>
        /// <param name="tenantId"><see cref="TenantId"/> that misses a stream processor.</param>
        public MissingScopedStreamProcessorTenant(TenantId tenantId) 
            : base($"Tenant '{tenantId}' does not have a scoped stream processor.")
        {
        }
    }
}

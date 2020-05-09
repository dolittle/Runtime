// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Exception that gets thrown when getting <see cref="IStreamDefinition" /> with <see cref="IStreamDefinitions.TryGet(ScopeId, StreamId, CancellationToken)" /> and the <see cref="IStreamDefinition" /> is not the same for all the Tenants.
    /// </summary>
    public class StreamDefinitionNotTheSameForAllTenants : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDefinitionNotTheSameForAllTenants"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        public StreamDefinitionNotTheSameForAllTenants(ScopeId scopeId, StreamId streamId)
            : base($"The Stream Definition for Stream: '{streamId}' in Scope: '{scopeId}' is not the same for all Tenants")
        {
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Exception that gets thrown when the processing of an embedding request is scheduled on the <see cref="EmbeddingProcessor"/> of the wrong tenant.
/// </summary>
public class EmbeddingRequestWorkScheduledForWrongTenant : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingRequestWorkScheduledForWrongTenant"/> class.
    /// </summary>
    /// <param name="requestTenant">The tenant that the request was for.</param>
    /// <param name="loopTenant">The tenant that the embedding processor is running for.</param>
    public EmbeddingRequestWorkScheduledForWrongTenant(TenantId requestTenant, TenantId loopTenant)
        : base($"The processing of an embedding request for tenant {requestTenant} was scheduled on the embedding processor for tenant {loopTenant}")
    {
    }
}

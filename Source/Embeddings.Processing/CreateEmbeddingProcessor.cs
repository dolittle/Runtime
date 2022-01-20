// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// A delegate represeting something that can create instances of <see cref="IEmbeddingProcessor"/> for a specific tenant.
/// </summary>
/// <param name="tenant">The <see cref="TenantId"/> to create a processor for.</param>
/// <returns>A <see cref="IEmbeddingProcessor"/> scoped to the specified tenant.</returns>
public delegate IEmbeddingProcessor CreateEmbeddingProcessorForTenant(TenantId tenant);
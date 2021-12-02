// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
/// runtime service implementations for Heads.
/// </summary>
public class RuntimeServices : ICanBindRuntimeServices
{
    readonly EmbeddingsService _embeddings;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
    /// </summary>
    /// <param name="embeddings">The <see cref="EmbeddingsService"/>.</param>
    public RuntimeServices(EmbeddingsService embeddings)
    {
        _embeddings = embeddings;
    }

    /// <inheritdoc/>
    public ServiceAspect Aspect => "Embeddings.Processing";

    /// <inheritdoc/>
    public IEnumerable<Service> BindServices() =>
        new[]
        {
            new Service(_embeddings, Contracts.Embeddings.BindService(_embeddings), Contracts.Embeddings.Descriptor)
        };
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a system that can provide metadata.
    /// </summary>
    public interface IMetadataProviders
    {
        /// <summary>
        /// Provide metadata from all providers.
        /// </summary>
        /// <returns><see cref="Metadata.Entry">Metadata entries</see> from all <see cref="ICanProvideClientMetadata">providers</see>.</returns>
        IEnumerable<Metadata.Entry> Provide();
    }
}
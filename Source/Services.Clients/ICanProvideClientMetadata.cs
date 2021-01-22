// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a system that is capable of providing metadata for client calls.
    /// </summary>
    public interface ICanProvideClientMetadata
    {
        /// <summary>
        /// Provide the necessary <see cref="Metadata.Entry">entries</see>.
        /// </summary>
        /// <returns><see cref="Metadata.Entry">Metadata entries</see>.</returns>
        IEnumerable<Metadata.Entry> Provide();
    }
}
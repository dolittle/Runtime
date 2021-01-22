// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Types;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetadataProviders"/>.
    /// </summary>
    public class MetadataProviders : IMetadataProviders
    {
        readonly IInstancesOf<ICanProvideClientMetadata> _metadataProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviders"/> class.
        /// </summary>
        /// <param name="metadataProviders"><see cref="IInstancesOf{T}"/> of <see cref="ICanProvideClientMetadata"/>.</param>
        public MetadataProviders(IInstancesOf<ICanProvideClientMetadata> metadataProviders)
        {
            _metadataProviders = metadataProviders;
        }

        /// <inheritdoc/>
        public IEnumerable<Metadata.Entry> Provide() =>
            _metadataProviders.SelectMany(_ => _.Provide());
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when trying to use an unsupported type in  <see cref="FilterDefinitionDiscriminatorConvention"/>.
    /// </summary>
    public class UnsupportedTypeForFilterDefinitionDiscriminatorConvention : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedTypeForFilterDefinitionDiscriminatorConvention"/> class.
        /// </summary>
        /// <param name="type">Nominal type used in the discriminator convention.</param>
        public UnsupportedTypeForFilterDefinitionDiscriminatorConvention(Type type)
            : base($"Type: {type} isn't derived from AbstractFilterDefinition and is not supported by {typeof(FilterDefinitionDiscriminatorConvention)}. Was this type erroneously registered with BsonSerializer.RegisterDiscriminatorConvention?", null)
        {
        }
    }
}

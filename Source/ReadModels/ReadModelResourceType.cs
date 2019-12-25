// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.ResourceTypes;

namespace Dolittle.ReadModels
{
    /// <summary>
    /// Represents a <see cref="IAmAResourceType"/> for read models.
    /// </summary>
    public class ReadModelResourceType : IAmAResourceType
    {
        /// <inheritdoc/>
        public ResourceType Name => "readModels";

        /// <inheritdoc/>
        public IEnumerable<Type> Services { get; } = new[]
        {
            typeof(IReadModelRepositoryFor<>),
            typeof(IAsyncReadModelRepositoryFor<>)
        };
    }
}
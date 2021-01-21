// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.IO.Tenants
{
    /// <summary>
    /// Represents a <see cref="IAmAResourceType">resource type</see> for <see cref="IFiles"/>.
    /// </summary>
    public class FilesResourceType : IAmAResourceType
    {
        /// <inheritdoc/>
        public ResourceType Name => "files";

        /// <inheritdoc/>
        public IEnumerable<Type> Services { get; } = new[] { typeof(IFiles) };
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Exception that gets thrown when there are duplicate resource representations.
    /// </summary>
    public class FoundDuplicateResourceDefinition : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FoundDuplicateResourceDefinition"/> class.
        /// </summary>
        /// <param name="resourceType">The <see cref="ResourceType"/> that had duplicate representations.</param>
        public FoundDuplicateResourceDefinition(ResourceType resourceType)
            : base($"Found one or more duplicate resource representations with {typeof(ResourceType).FullName} {resourceType.Value}.")
        {
        }
    }
}
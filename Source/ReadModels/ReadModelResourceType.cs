/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Dolittle.ResourceTypes;

namespace Dolittle.ReadModels
{
    /// <inheritdoc/>
    public class ReadModelResourceType : IAmAResourceType
    {
        readonly IEnumerable<Type> _services = new []{typeof(IReadModelRepositoryFor<>)};
        
        /// <inheritdoc/>
        public ResourceType Name => "readModels";
        /// <inheritdoc/>
        public IEnumerable<Type> Services => _services;
    }
}
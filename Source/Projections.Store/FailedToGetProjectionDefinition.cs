// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Projections.Store
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="ProjectionDefinition" /> could not be retrieved.
    /// </summary>
    public class FailedToGetProjectionDefinition : Exception
    {
        /// <summary>
        /// Initializes an instance of the <see cref="FailedToGetProjectionDefinition" /> class.
        /// </summary>
        /// <param name="projection">The projection id.</param>
        /// <param name="scope">The scope id.</param>
        public FailedToGetProjectionDefinition(ProjectionId projection, ScopeId scope)
            : base($"Failed to get projection definition for projection {projection.Value} in scope {scope.Value}")
        {
        }
    }
}
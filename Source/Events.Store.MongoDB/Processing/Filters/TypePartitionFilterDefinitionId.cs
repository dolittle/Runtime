// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents the id of a <see cref="TypePartitionFilterDefinition" />.
    /// </summary>
    public class TypePartitionFilterDefinitionId
    {
        /// <summary>
        /// Gets or sets the source stream id.
        /// </summary>
        public Guid SourceStream { get; set; }

        /// <summary>
        /// Gets or sets the target stream id.
        /// </summary>
        public Guid TargetStream { get; set; }
    }
}
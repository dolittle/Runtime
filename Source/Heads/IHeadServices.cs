// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Defines a system that holds information about all head services exposed by a connected head.
    /// </summary>
    public interface IHeadServices
    {
        /// <summary>
        /// Gets the <see cref="HeadServiceDefinition"/> for all exposed services.
        /// </summary>
        IEnumerable<HeadServiceDefinition> Services { get; }
    }
}
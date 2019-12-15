// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Lifecycle;
using Dolittle.Types;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents an implementation of <see cref="IHeadServices"/>.
    /// </summary>
    [Singleton]
    public class HeadServices : IHeadServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeadServices"/> class.
        /// </summary>
        /// <param name="definers">Instances of <see cref="IDefineHeadServices">definers</see>.</param>
        public HeadServices(IInstancesOf<IDefineHeadServices> definers)
        {
            Services = definers.SelectMany(_ => _.Services);
        }

        /// <inheritdoc/>
        public IEnumerable<HeadServiceDefinition> Services { get; }
    }
}
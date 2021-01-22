// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="IContainer"/> is not set yet.
    /// </summary>
    public class ContainerNotSetYet : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerNotSetYet"/> class.
        /// </summary>
        public ContainerNotSetYet()
            : base("Container has not been set yet ")
        {
        }
    }
}
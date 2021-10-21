// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime
{
    /// <summary>
    /// Exception that gets thrown when unable to decide a Runtime address to connect to.
    /// </summary>
    public class CouldNotFindRuntimeAddress : Exception
    {
        public CouldNotFindRuntimeAddress()
            : base("No Runtimes discovered, and no address was provided as an argument.")
        {
        }
    }
}

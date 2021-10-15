// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Microservices;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime
{
    /// <summary>
    /// A shared command base for the "dolittle runtime" commands that provides shared arguments.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// The "--runtime" argument used to provide an address for to a Runtime to connect to.
        /// </summary>
        [Option("--runtime", CommandOptionType.SingleValue, Description = "The <host[:port]> to use to connect to the management endpoint of a Runtime")]
        public MicroserviceAddress Runtime { get; init; }
    }
}
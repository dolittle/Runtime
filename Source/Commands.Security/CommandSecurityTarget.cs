// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Security;

namespace Dolittle.Runtime.Commands.Security
{
    /// <summary>
    /// Represents a specific <see cref="ISecurityTarget"/> for <see cref="CommandRequest">commands</see>.
    /// </summary>
    public class CommandSecurityTarget : SecurityTarget
    {
        const string COMMAND = "Command";

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSecurityTarget"/> class.
        /// </summary>
        public CommandSecurityTarget()
            : base(COMMAND)
        {
        }
    }
}

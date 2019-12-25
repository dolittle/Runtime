// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Security;

namespace Dolittle.Runtime.Commands.Security
{
    /// <summary>
    /// Represents a <see cref="ISecurityAction"/> for handling <see cref="CommandRequest">commands</see>.
    /// </summary>
    public class HandleCommand : SecurityAction
    {
        /// <inheritdoc/>
        public override string ActionType => "Handle";
    }
}

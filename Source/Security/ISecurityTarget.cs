// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Defines a <see cref="ISecurityTarget"/>.
    /// </summary>
    public interface ISecurityTarget
    {
        /// <summary>
        /// Gets all <see cref="ISecurable">securables</see>.
        /// </summary>
        IEnumerable<ISecurable> Securables { get; }

        /// <summary>
        /// Gets a description of the SecurityTarget.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Add a <see cref="ISecurable"/>.
        /// </summary>
        /// <param name="securable"><see cref="ISecurityActor"/> to add.</param>
        void AddSecurable(ISecurable securable);

        /// <summary>
        /// Indicates whether this target can authorize the instance of this action.
        /// </summary>
        /// <param name="actionToAuthorize">An instance of the action to authorize.</param>
        /// <returns>True if the <see cref="ISecurityTarget"/> can authorize this action, False otherwise.</returns>
        bool CanAuthorize(object actionToAuthorize);

        /// <summary>
        /// Authorizes this <see cref="ISecurityTarget"/> for the instance of the action.
        /// </summary>
        /// <param name="actionToAuthorize">Instance of an action to be authorized.</param>
        /// <returns>An <see cref="AuthorizeTargetResult"/> that indicates if the action was authorized or not.</returns>
        AuthorizeTargetResult Authorize(object actionToAuthorize);
    }
}

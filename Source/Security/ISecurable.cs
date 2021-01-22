// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Defines something that can be secured.
    /// </summary>
    public interface ISecurable
    {
        /// <summary>
        /// Gets a collection of <see cref="ISecurityActor">security objects</see>.
        /// </summary>
        IEnumerable<ISecurityActor> Actors { get; }

        /// <summary>
        /// Gets a description of the Securable.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Add a <see cref="ISecurityActor"/> as context for rules.
        /// </summary>
        /// <param name="actor"><see cref="ISecurityActor"/> to add.</param>
        void AddActor(ISecurityActor actor);

        /// <summary>
        /// Indicates whether this securable can authorize the action.
        /// </summary>
        /// <param name="actionToAuthorize">Instance of an action to be authorized.</param>
        /// <returns>True for can authorize, False for cannot.</returns>
        bool CanAuthorize(object actionToAuthorize);

        /// <summary>
        /// Result of the authorization of this securable.
        /// </summary>
        /// <param name="actionToAuthorize">Instance of an action to be authorized.</param>
        /// <returns>An <see cref="AuthorizeSecurableResult"/>.</returns>
        AuthorizeSecurableResult Authorize(object actionToAuthorize);
    }
}

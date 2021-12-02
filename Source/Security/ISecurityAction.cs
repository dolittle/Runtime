// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Defines an action that is subject to security.
/// </summary>
public interface ISecurityAction
{
    /// <summary>
    /// Gets all <see cref="ISecurityTarget">security targets</see>.
    /// </summary>
    IEnumerable<ISecurityTarget> Targets { get; }

    /// <summary>
    /// Gets a string description of this <see cref="ISecurityAction"/>.
    /// </summary>
    string ActionType { get; }

    /// <summary>
    /// Add a <see cref="ISecurityTarget"/>.
    /// </summary>
    /// <param name="securityTarget"><see cref="ISecurityTarget"/> to add.</param>
    void AddTarget(ISecurityTarget securityTarget);

    /// <summary>
    /// Indicates whether this action can authorize the instance of the action.
    /// </summary>
    /// <param name="actionToAuthorize">An instance of the action to authorize.</param>
    /// <returns>True if the <see cref="ISecurityAction"/> can authorize this action, False otherwise.</returns>
    bool CanAuthorize(object actionToAuthorize);

    /// <summary>
    /// Authorizes this <see cref="ISecurityAction"/> for the instance of the action.
    /// </summary>
    /// <param name="actionToAuthorize">Instance of an action to be authorized.</param>
    /// <returns>An <see cref="AuthorizeActionResult"/> that indicates if the action was authorized or not.</returns>
    AuthorizeActionResult Authorize(object actionToAuthorize);
}
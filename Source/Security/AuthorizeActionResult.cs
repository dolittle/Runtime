// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Represents the result of an authorization of a <see cref="ISecurityAction"/>.
/// </summary>
public class AuthorizeActionResult
{
    readonly List<AuthorizeTargetResult> _authorizationFailures = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeActionResult"/> class.
    /// </summary>
    /// <param name="action"><see cref="ISecurityAction"/> that this <see cref="AuthorizeActionResult"/> pertains to.</param>
    public AuthorizeActionResult(ISecurityAction action)
    {
        Action = action;
    }

    /// <summary>
    /// Gets the <see cref="ISecurityAction"/> that this <see cref="AuthorizeTargetResult"/> pertains to.
    /// </summary>
    public ISecurityAction Action { get; }

    /// <summary>
    /// Gets a value indicating whether indicates the Authorization attempt was successful or not.
    /// </summary>
    public virtual bool IsAuthorized => _authorizationFailures.Count == 0;

    /// <summary>
    /// Gets the <see cref="AuthorizeTargetResult"/> for all failed <see cref="ISecurityTarget"> Actors </see> on the <see cref="ISecurable"/>.
    /// </summary>
    public IEnumerable<AuthorizeTargetResult> AuthorizationFailures => _authorizationFailures.AsEnumerable();

    /// <summary>
    /// Processes an <see cref="AuthorizeTargetResult"/> for an <see cref="ISecurityTarget"> Actor</see> adding it to the AuthorizationFailures collection if appropriate.
    /// </summary>
    /// <param name="result">Result to process.</param>
    public void ProcessAuthorizeTargetResult(AuthorizeTargetResult result)
    {
        if (!result.IsAuthorized)
            _authorizationFailures.Add(result);
    }

    /// <summary>
    /// Builds a collection of strings that show Action/Target for each broken or erroring rule in <see cref="AuthorizeActionResult"/>.
    /// </summary>
    /// <returns>A collection of strings.</returns>
    public virtual IEnumerable<string> BuildFailedAuthorizationMessages()
    {
        return from result in AuthorizationFailures from message in result.BuildFailedAuthorizationMessages() select Action.ActionType + "/" + message;
    }
}
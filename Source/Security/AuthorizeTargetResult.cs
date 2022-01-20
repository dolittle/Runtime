// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Represents the result of an authorization of a <see cref="ISecurityTarget"/>.
/// </summary>
public class AuthorizeTargetResult
{
    readonly List<AuthorizeSecurableResult> _authorizationFailures = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeTargetResult"/> class.
    /// </summary>
    /// <param name="target"><see cref="ISecurityTarget"/> that this <see cref="AuthorizeTargetResult"/> pertains to.</param>
    public AuthorizeTargetResult(ISecurityTarget target)
    {
        Target = target;
    }

    /// <summary>
    /// Gets the <see cref="ISecurityTarget"/> that this <see cref="AuthorizeTargetResult"/> pertains to.
    /// </summary>
    public ISecurityTarget Target { get; }

    /// <summary>
    /// Gets the <see cref="AuthorizeSecurableResult"/> for each failed <see cref="ISecurable"/> on the <see cref="ISecurityTarget"/>.
    /// </summary>
    public IEnumerable<AuthorizeSecurableResult> AuthorizationFailures => _authorizationFailures.AsEnumerable();

    /// <summary>
    /// Gets a value indicating whether indicates the Authorization attempt was successful or not.
    /// </summary>
    public virtual bool IsAuthorized => !AuthorizationFailures.Any();

    /// <summary>
    /// Processes an <see cref="AuthorizeSecurableResult"/>, adding it to the collection of AuthorizationFailures if appropriate.
    /// </summary>
    /// <param name="result">An <see cref="AuthorizeSecurableResult"/> for a <see cref="ISecurable"/>.</param>
    public void ProcessAuthorizeSecurableResult(AuthorizeSecurableResult result)
    {
        if (!result.IsAuthorized)
            _authorizationFailures.Add(result);
    }

    /// <summary>
    /// Builds a collection of strings that show Target/Securable for each broken or erroring rule in<see cref="AuthorizeTargetResult"/>.
    /// </summary>
    /// <returns>A collection of strings.</returns>
    public virtual IEnumerable<string> BuildFailedAuthorizationMessages()
    {
        return from result in AuthorizationFailures from message in result.BuildFailedAuthorizationMessages() select Target.Description + "/" + message;
    }
}
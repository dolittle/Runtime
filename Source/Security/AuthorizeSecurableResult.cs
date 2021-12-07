﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Represents the result of an authorization of a <see cref="ISecurable"/>.
/// </summary>
public class AuthorizeSecurableResult
{
    readonly List<AuthorizeActorResult> _authorizationFailures = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeSecurableResult"/> class.
    /// </summary>
    /// <param name="securable"><see cref="ISecurable"/> that this <see cref="AuthorizeSecurableResult"/> pertains to.</param>
    public AuthorizeSecurableResult(ISecurable securable)
    {
        Securable = securable;
    }

    /// <summary>
    /// Gets the <see cref="ISecurable"/> that this <see cref="AuthorizeSecurableResult"/> pertains to.
    /// </summary>
    public ISecurable Securable { get; }

    /// <summary>
    /// Gets the <see cref="AuthorizeActorResult"/> for all failed <see cref="ISecurityActor"> Actors </see> on the <see cref="ISecurable"/>.
    /// </summary>
    public IEnumerable<AuthorizeActorResult> AuthorizationFailures => _authorizationFailures.AsEnumerable();

    /// <summary>
    /// Gets a value indicating whether indicates the Authorization attempt was successful or not.
    /// </summary>
    public virtual bool IsAuthorized => !AuthorizationFailures.Any();

    /// <summary>
    /// Processes an <see cref="AuthorizeActorResult"/> for an <see cref="ISecurityActor"> Actor</see>, adding it to the AuthorizationFailures collection if appropriate.
    /// </summary>
    /// <param name="authorizeActorResult">Result to process.</param>
    public void ProcessAuthorizeActorResult(AuthorizeActorResult authorizeActorResult)
    {
        if (!authorizeActorResult.IsAuthorized)
        {
            _authorizationFailures.Add(authorizeActorResult);
        }
    }

    /// <summary>
    /// Builds a collection of strings that show Securable/Actor for each broken or erroring rule in <see cref="AuthorizeSecurableResult"/>.
    /// </summary>
    /// <returns>A collection of strings.</returns>
    public virtual IEnumerable<string> BuildFailedAuthorizationMessages()
    {
        return from result in AuthorizationFailures from message in result.BuildFailedAuthorizationMessages() select Securable.Description + "/" + message;
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Represents the result of an authorization attempt.
/// </summary>
public class AuthorizeDescriptorResult
{
    readonly List<AuthorizeActionResult> _authorizationFailures = new();

    /// <summary>
    /// Gets any <see cref="AuthorizeActionResult"> results</see> that were not authorized.
    /// </summary>
    public IEnumerable<AuthorizeActionResult> AuthorizationFailures => _authorizationFailures.AsEnumerable();

    /// <summary>
    /// Gets a value indicating whether indicates the Authorization attempt was successful or not.
    /// </summary>
    public virtual bool IsAuthorized => _authorizationFailures.Count == 0;

    /// <summary>
    /// Processes instance of an <see cref="AuthorizeActionResult"/>, adding failed authorizations to the AuthorizationFailures collection.
    /// </summary>
    /// <param name="result">Result to process.</param>
    public void ProcessAuthorizeActionResult(AuthorizeActionResult result)
    {
        if (!result.IsAuthorized)
        {
            _authorizationFailures.Add(result);
        }
    }

    /// <summary>
    /// Gets all the broken <see cref="ISecurityRule">rules</see> for this authorization attempt.
    /// </summary>
    /// <returns>A string describing each broken rule or an empty enumerable if there are none.</returns>
    public virtual IEnumerable<string> BuildFailedAuthorizationMessages()
    {
        var messages = new List<string>();
        foreach (var result in AuthorizationFailures)
        {
            messages.AddRange(result.BuildFailedAuthorizationMessages());
        }

        return messages;
    }
}
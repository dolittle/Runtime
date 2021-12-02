// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Security;

/// <summary>
/// Defines a rule for security.
/// </summary>
public interface ISecurityRule
{
    /// <summary>
    /// Gets a description of the rule.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Check if a securable instance is authorized.
    /// </summary>
    /// <param name="securable">The securable instance to check.</param>
    /// <returns>True if has access, false if not.</returns>
    bool IsAuthorized(object securable);
}
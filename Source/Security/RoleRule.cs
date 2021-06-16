// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a specific <see cref="ISecurityRule"/> for roles.
    /// </summary>
    public class RoleRule : ISecurityRule
    {
        /// <summary>
        /// The format string for describing the rule.
        /// </summary>
        public const string DescriptionFormat = "RequiredRole_{{{0}}}";

        readonly IUserSecurityActor _userToAuthorize;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRule"/> class.
        /// </summary>
        /// <param name="userToAuthorize">The <see cref="UserSecurityActor" /> to check the role against.</param>
        /// <param name="role">The role to check for.</param>
        public RoleRule(IUserSecurityActor userToAuthorize, string role)
        {
            _userToAuthorize = userToAuthorize;
            Role = role;
        }

        /// <summary>
        /// Gets the role for the rule.
        /// </summary>
        public string Role { get; }

        /// <inheritdoc/>
        public string Description => string.Format(CultureInfo.InvariantCulture, DescriptionFormat, Role);

        /// <inheritdoc/>
        public bool IsAuthorized(object securable)
        {
            return string.IsNullOrWhiteSpace(Role) || _userToAuthorize.IsInRole(Role);
        }
    }
}

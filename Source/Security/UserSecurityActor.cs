// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a concrete <see cref="SecurityActor"/> for a user.
    /// </summary>
    public class UserSecurityActor : SecurityActor, IUserSecurityActor
    {
        /// <summary>
        /// Description of the <see cref="UserSecurityActor"/>.
        /// </summary>
        public const string USER = "User";

        readonly ICanResolvePrincipal _resolvePrincipal;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSecurityActor"/> class.
        /// </summary>
        /// <param name="resolvePrincipal"><see cref="ICanResolvePrincipal"/> for resolving the principal.</param>
        public UserSecurityActor(ICanResolvePrincipal resolvePrincipal)
            : base(USER)
        {
            _resolvePrincipal = resolvePrincipal;
        }

        /// <inheritdoc/>
        public bool IsInRole(string role)
        {
            return _resolvePrincipal.Resolve().IsInRole(role);
        }

        /// <inheritdoc/>
        public bool HasClaimType(string claimType)
        {
            return _resolvePrincipal.Resolve().FindAll(claimType).Any();
        }

        /// <inheritdoc/>
        public bool HasClaimTypeWithValue(string claimType, string value)
        {
            return _resolvePrincipal.Resolve().Claims.Any(_ => _.Type == claimType && _.Value == value);
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a base for any <see cref="ISecurityAction"/>.
    /// </summary>
    public class SecurityAction : ISecurityAction
    {
        readonly List<ISecurityTarget> _targets = new();

        /// <inheritdoc/>
        public virtual string ActionType => string.Empty;

        /// <inheritdoc/>
        public IEnumerable<ISecurityTarget> Targets => _targets.AsEnumerable();

        /// <inheritdoc/>
        public void AddTarget(ISecurityTarget securityTarget)
        {
            _targets.Add(securityTarget);
        }

        /// <inheritdoc/>
        public bool CanAuthorize(object actionToAuthorize)
        {
            return _targets.Any(s => s.CanAuthorize(actionToAuthorize));
        }

        /// <inheritdoc/>
        public AuthorizeActionResult Authorize(object actionToAuthorize)
        {
            var result = new AuthorizeActionResult(this);
            foreach (var target in Targets.Where(t => t.CanAuthorize(actionToAuthorize)))
            {
                result.ProcessAuthorizeTargetResult(target.Authorize(actionToAuthorize));
            }

            return result;
        }
    }
}

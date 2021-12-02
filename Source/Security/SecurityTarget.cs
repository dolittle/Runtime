// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a base class for any <see cref="ISecurityTarget">security targets</see>.
    /// </summary>
    public class SecurityTarget : ISecurityTarget
    {
        readonly List<ISecurable> _securables = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityTarget"/> class.
        /// </summary>
        /// <param name="description">A description for this <see cref="SecurityTarget"/>.</param>
        public SecurityTarget(string description)
        {
            Description = description ?? string.Empty;
        }

        /// <inheritdoc/>
        public IEnumerable<ISecurable> Securables => _securables;

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public void AddSecurable(ISecurable securityObject)
        {
            _securables.Add(securityObject);
        }

        /// <inheritdoc/>
        public bool CanAuthorize(object actionToAuthorize)
        {
            return _securables.Any(s => s.CanAuthorize(actionToAuthorize));
        }

        /// <inheritdoc/>
        public AuthorizeTargetResult Authorize(object actionToAuthorize)
        {
            var result = new AuthorizeTargetResult(this);
            foreach (var securable in Securables)
            {
                result.ProcessAuthorizeSecurableResult(securable.Authorize(actionToAuthorize));
            }

            return result;
        }
    }
}

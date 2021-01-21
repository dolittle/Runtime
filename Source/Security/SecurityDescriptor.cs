// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a <see cref="ISecurityDescriptor"/>.
    /// </summary>
    public class SecurityDescriptor : ISecurityDescriptor
    {
        readonly List<ISecurityAction> _actions = new List<ISecurityAction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityDescriptor"/> class.
        /// </summary>
        public SecurityDescriptor()
        {
            When = new SecurityDescriptorBuilder(this);
        }

        /// <inheritdoc/>
        public ISecurityDescriptorBuilder When { get; }

        /// <inheritdoc/>
        public IEnumerable<ISecurityAction> Actions => _actions;

        /// <inheritdoc/>
        public void AddAction(ISecurityAction securityAction)
        {
            _actions.Add(securityAction);
        }

        /// <inheritdoc/>
        public bool CanAuthorize<T>(object instanceToAuthorize)
            where T : ISecurityAction
        {
            return _actions.Any(a => a.GetType() == typeof(T) && a.CanAuthorize(instanceToAuthorize));
        }

        /// <inheritdoc/>
        public AuthorizeDescriptorResult Authorize(object instanceToAuthorize)
        {
            var result = new AuthorizeDescriptorResult();
            foreach (var action in Actions.Where(a => a.CanAuthorize(instanceToAuthorize)))
            {
                result.ProcessAuthorizeActionResult(action.Authorize(instanceToAuthorize));
            }

            return result;
        }
    }
}

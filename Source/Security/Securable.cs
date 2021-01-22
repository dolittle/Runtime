// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a base implementation of<see cref="ISecurable"/>.
    /// </summary>
    public class Securable : ISecurable
    {
        readonly List<ISecurityActor> _actors = new List<ISecurityActor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Securable"/> class.
        /// </summary>
        /// <param name="securableDescription">Description of the Securable.</param>
        public Securable(string securableDescription)
        {
            Description = securableDescription ?? string.Empty;
        }

        /// <inheritdoc/>
        public IEnumerable<ISecurityActor> Actors => _actors;

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public void AddActor(ISecurityActor actor)
        {
            _actors.Add(actor);
        }

        /// <inheritdoc/>
        public virtual bool CanAuthorize(object actionToAuthorize)
        {
            return false;
        }

        /// <inheritdoc/>
        public virtual AuthorizeSecurableResult Authorize(object actionToAuthorize)
        {
            var result = new AuthorizeSecurableResult(this);
            foreach (var actor in _actors)
            {
                result.ProcessAuthorizeActorResult(actor.IsAuthorized(actionToAuthorize));
            }

            return result;
        }
    }
}

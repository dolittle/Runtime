// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents an implementation of <see cref="ISecurityActor"/>.
    /// </summary>
    public class SecurityActor : ISecurityActor
    {
        readonly List<ISecurityRule> _rules = new List<ISecurityRule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityActor"/> class.
        /// </summary>
        /// <param name="description">String that describes this actor type.</param>
        public SecurityActor(string description)
        {
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// Gets all the <see cref="ISecurityRule">security rules</see>.
        /// </summary>
        public IEnumerable<ISecurityRule> Rules => _rules;

        /// <inheritdoc/>
        public string Description { get; }

        /// <summary>
        /// Add a <see cref="ISecurityRule"/> to the <see cref="SecurityActor"/>.
        /// </summary>
        /// <param name="rule"><see cref="ISecurityRule"/> to add.</param>
        public void AddRule(ISecurityRule rule)
        {
            _rules.Add(rule);
        }

        /// <inheritdoc/>
        public AuthorizeActorResult IsAuthorized(object actionToAuthorize)
        {
            var result = new AuthorizeActorResult(this);
            foreach (var rule in _rules)
            {
                try
                {
                    if (!rule.IsAuthorized(actionToAuthorize))
                        result.AddBrokenRule(rule);
                }
                catch (Exception ex)
                {
                    result.AddErrorRule(rule, ex);
                }
            }

            return result;
        }
    }
}

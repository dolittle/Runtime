// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents a broken rule.
    /// </summary>
    public class BrokenRule
    {
        readonly List<Cause> _causes = new List<Cause>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenRule"/> class.
        /// </summary>
        /// <param name="rule"><see cref="IRule"/> that is broken.</param>
        /// <param name="instance">Instance related to the broken rule when evaluated.</param>
        /// <param name="context"><see cref="IRuleContext"/> rule was running in.</param>
        public BrokenRule(IRule rule, object instance, IRuleContext context)
        {
            Rule = rule;
            Instance = instance;
            Context = context;
        }

        /// <summary>
        /// Gets the type of rule that is broken.
        /// </summary>
        public IRule Rule { get; }

        /// <summary>
        /// Gets the instance used for evaluating the <see cref="IRule"/>.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// Gets the context in which the rule was broken.
        /// </summary>
        public IRuleContext Context { get; }

        /// <summary>
        /// Gets the <see cref="Cause">causes</see> why the rule is broken.
        /// </summary>
        public IEnumerable<Cause> Causes => _causes.ToArray();

        /// <summary>
        /// Add a reason for the <see cref="IRule"/> being broken.
        /// </summary>
        /// <param name="cause"><see cref="Cause"/>.</param>
        public void AddCause(Cause cause) => _causes.Add(cause);
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Specifications
{
    /// <summary>
    /// Base class for expressing a complex rule in code. Utilizing the Specification pattern.
    /// </summary>
    /// <typeparam name="T">Type that the rule applies to.</typeparam>
    /// <remarks>
    /// Based on http://bloggingabout.net/blogs/dries/archive/2011/09/29/specification-pattern-continued.aspx.
    /// </remarks>
    public abstract class Specification<T>
    {
        /// <summary>
        /// Gets or sets predicate rule to be evaluated.
        /// </summary>
        protected internal Expression<Func<T, bool>> Predicate
        {
            get
            {
                return EvalExpression;
            }

            set
            {
                EvalExpression = value;
                EvalCompiled = EvalExpression.Compile();
            }
        }

        /// <summary>
        /// Gets  the compiled predicate for use against an instance.
        /// </summary>
        protected Func<T, bool> EvalCompiled { get; private set; }

        /// <summary>
        /// Gets the predicate as an expression for use against IQueryable collection.
        /// </summary>
        protected Expression<Func<T, bool>> EvalExpression { get; private set; }

        /// <summary>
        /// Evalutes the rule against a single instance of type T.
        /// </summary>
        /// <param name="instance">The instance to evaluation the rule against.</param>
        /// <returns>true if the rule is satisfied, false if the rule is broken.</returns>
        public bool IsSatisfiedBy(T instance) => EvalCompiled(instance);

        /// <summary>
        /// Evaluates the rule against each instance of an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="candidates">The <see cref="IQueryable{T}"/> that will have the rule evaluated against each of its instances.</param>
        /// <returns>An <see cref="IQueryable{T}"/> containing only instances that satisfy the rule.</returns>
        public IQueryable<T> SatisfyingElementsFrom(IQueryable<T> candidates) => candidates.Where(EvalExpression);
    }
}

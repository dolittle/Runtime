// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Specifications
{
    /// <summary>
    /// Extensions to help chain simple rules into complex rules.
    /// </summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Combines two atomic rules into a single rule.
        /// </summary>
        /// <typeparam name="T">Type of the instance that the rule is to be evaluated against.</typeparam>
        /// <param name="lhs">The rule being extended.</param>
        /// <param name="rhs">The second rule to be merged into the first.</param>
        /// <param name="merge">Expression for merging the two rules.</param>
        /// <returns>The composed <see cref="Specification{T}"/>.</returns>
        public static Specification<T> Compose<T>(this Specification<T> lhs, Specification<T> rhs, Func<Expression, Expression, Expression> merge)
            => new CompositeRule<T>(lhs, rhs, merge);


        /// <summary>
        /// Combines two rules in to an "And" rule.
        /// </summary>
        /// <typeparam name="T">Type of the instance that the rule is to be evaluated against.</typeparam>
        /// <param name="lhs">The rule being extended.</param>
        /// <param name="rhs">The second rule to be merged into the first.</param>
        /// <returns>An And{T} rule">.</returns>
        public static Specification<T> And<T>(this Specification<T> lhs, Specification<T> rhs)
            => new And<T>(lhs, rhs);

        /// <summary>
        /// Combines two rules in to an "And" rul, where the second rule is negated.
        /// </summary>
        /// <typeparam name="T">Type of the instance that the rule is to be evaluated against.</typeparam>
        /// <param name="lhs">The rule being extended.</param>
        /// <param name="rhs">The second rule to be merged into the first.</param>
        /// <returns>An And{T} rule">.</returns>
        public static Specification<T> AndNot<T>(this Specification<T> lhs, Specification<T> rhs)
            => new And<T>(lhs, Is.Not(rhs));

        /// <summary>
        /// Combines two rules in to an "Or" rule.
        /// </summary>
        /// <typeparam name="T">Type of the instance that the rule is to be evaluated against.</typeparam>
        /// <param name="lhs">The rule being extended.</param>
        /// <param name="rhs">The second rule to be merged into the first.</param>
        /// <returns>An Or{T} rule">.</returns>
        public static Specification<T> Or<T>(this Specification<T> lhs, Specification<T> rhs)
            => new Or<T>(lhs, rhs);

        /// <summary>
        /// Combines two rules into an Or, where the second rule is negated.
        /// </summary>
        /// <typeparam name="T">Type of the instance that the rule is to be evaluated against.</typeparam>
        /// <param name="rhs">The rule being extended.</param>
        /// <param name="lhs">The second rule to be merged into the first.</param>
        /// <returns>An And{T} rule">.</returns>
        public static Specification<T> OrNot<T>(this Specification<T> rhs, Specification<T> lhs)
            => new Or<T>(rhs, Is.Not(lhs));
    }
}
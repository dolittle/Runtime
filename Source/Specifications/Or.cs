// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Dolittle.Runtime.Specifications
{
    /// <summary>
    /// Composes a rule that will be satisfied if either the first rule or second rule is satisfied.
    /// </summary>
    /// <typeparam name="T">Type that the rule is to be evaluated for.</typeparam>
    /// <remarks>Based on http://bloggingabout.net/blogs/dries/archive/2011/09/29/specification-pattern-continued.aspx.</remarks>
    class Or<T> : Specification<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Or{T}"/> class.
        /// </summary>
        /// <param name="leftHandSide">The <see cref="Specification{T}"/> for the left hand side.</param>
        /// <param name="rightHandSide">The <see cref="Specification{T}"/> for the right hand side.</param>
        internal Or(Specification<T> leftHandSide, Specification<T> rightHandSide)
            => Predicate = leftHandSide.Compose(rightHandSide, Expression.Or).Predicate;
    }
}
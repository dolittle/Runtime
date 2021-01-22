// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Specifications
{
    /// <summary>
    /// Negates a rule.  Rule is satisfied if the provided rule is not satisfied.
    /// </summary>
    /// <typeparam name="T">Type that the rule is to be evalued for.</typeparam>
    /// <remarks>Based on http://bloggingabout.net/blogs/dries/archive/2011/09/29/specification-pattern-continued.aspx.</remarks>
    internal class Negative<T> : Specification<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Negative{T}"/> class.
        /// </summary>
        /// <param name="specification">The <see cref="Specification{T}"/> to negate.</param>
        internal Negative(Specification<T> specification)
        {
            Predicate = Expression.Lambda<Func<T, bool>>(Expression.Not(specification.Predicate.Body), specification.Predicate.Parameters);
        }
    }
}
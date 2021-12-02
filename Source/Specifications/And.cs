// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Dolittle.Runtime.Specifications;

/// <summary>
/// Composes a rule that will be satisfied if both the first rule and the second rule are satisfied.
/// </summary>
/// <typeparam name="T">Type that the rule is to be evaluated for.</typeparam>
/// <remarks>Based on http://bloggingabout.net/blogs/dries/archive/2011/09/29/specification-pattern-continued.aspx.</remarks>
class And<T> : Specification<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="And{T}"/> class.
    /// </summary>
    /// <param name="leftHandSide">The <see cref="Specification{T}"/> for the left hand side.</param>
    /// <param name="rightHandSide">The <see cref="Specification{T}"/> for the right hand side.</param>
    internal And(Specification<T> leftHandSide, Specification<T> rightHandSide)
        => Predicate = rightHandSide.Compose(leftHandSide, Expression.And).Predicate;
}
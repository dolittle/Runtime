// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Specifications;

/// <summary>
/// Composes two rules into a single rule that can be evaluated atomically.
/// </summary>
/// <typeparam name="T">Type that the rule is to be evaluated for.</typeparam>
/// <remarks>Based on http://bloggingabout.net/blogs/dries/archive/2011/09/29/specification-pattern-continued.aspx.</remarks>
class CompositeRule<T> : Specification<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeRule{T}"/> class.
    /// </summary>
    /// <param name="leftHandSide">The <see cref="Specification{T}"/> for the left hand side.</param>
    /// <param name="rightHandSide">The <see cref="Specification{T}"/> for the right hand side.</param>
    /// <param name="merge">The <see cref="Func{T1,T2,TOut}"/> that deals with the merge.</param>
    internal CompositeRule(
        Specification<T> leftHandSide,
        Specification<T> rightHandSide,
        Func<Expression, Expression, Expression> merge)
    {
        var map = leftHandSide.Predicate.Parameters.Select((f, i) => (f, s: rightHandSide.Predicate.Parameters[i])).ToDictionary(p => p.s, p => p.f);
        var secondBody = ParameterRebinder.ReplaceParameters(map, rightHandSide.Predicate.Body);
        Predicate = Expression.Lambda<Func<T, bool>>(merge(leftHandSide.Predicate.Body, secondBody), leftHandSide.Predicate.Parameters);
    }
}
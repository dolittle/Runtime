// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Specifications;

/// <summary>
/// Helper for combining Lambdas.
/// Ensures that the combined expression points at the same parameters where these are common.
/// </summary>
/// <remarks>Based on http://bloggingabout.net/blogs/dries/archive/2011/09/29/specification-pattern-continued.aspx.</remarks>
class ParameterRebinder : ExpressionVisitor
{
    readonly Dictionary<ParameterExpression, ParameterExpression> _map;

    ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        => _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

    /// <summary>
    /// Replace expression by visitation with an expression.
    /// </summary>
    /// <param name="map"><see cref="Dictionary{TKey,TValue}"/>r representing the map.</param>
    /// <param name="expression"><see cref="Expression"/> to visit with.</param>
    /// <returns>A new <see cref="Expression"/>.</returns>
    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression expression)
        => new ParameterRebinder(map).Visit(expression);

    /// <inheritdoc/>
    protected override Expression VisitParameter(ParameterExpression parameterExpression)
    {
        if (_map.TryGetValue(parameterExpression, out var replacement))
        {
            parameterExpression = replacement;
        }
        return base.VisitParameter(parameterExpression);
    }
}
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation.Rules
{
    /// <summary>
    /// Represents the <see cref="ValueRule"/> for specific regular expression - any value must conform with a regular expression.
    /// </summary>
    public class Regex : ValueRule
    {
        /// <summary>
        /// When a string does not conform to the specified expression, this is the reason given.
        /// </summary>
        public static Reason NotConformingToExpression = Reason.Create("BE58A125-40DB-47EA-B260-37F7AF4455C5", "Value '{Value}' does not conform to regular expression");

        readonly System.Text.RegularExpressions.Regex _actualRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Regex"/> class.
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for.</param>
        /// <param name="expression">Regular Expression to use.</param>
        public Regex(PropertyInfo property, string expression)
            : base(property)
        {
            Expression = expression;
            _actualRegex = new System.Text.RegularExpressions.Regex(expression);
        }

        /// <summary>
        /// Gets the expression that values must conform to.
        /// </summary>
        public string Expression { get; }

        /// <inheritdoc/>
        public override void Evaluate(IRuleContext context, object instance)
        {
            if (FailIfValueTypeMismatch<string>(context, instance))
            {
                if (!_actualRegex.IsMatch((string)instance)) context.Fail(this, instance, NotConformingToExpression.WithArgs(new { ValueRule = instance }));
            }
        }
    }
}

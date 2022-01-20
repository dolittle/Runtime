// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Encapsulates a <see cref="ISecurityRule"/> that encountered an Exception when evaluating.
/// </summary>
public class RuleEvaluationError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEvaluationError"/> class.
    /// </summary>
    /// <param name="rule"><see cref="ISecurityRule"/> that encounted the error when evaluating.</param>
    /// <param name="error">The error that was encountered.</param>
    public RuleEvaluationError(ISecurityRule rule, Exception error)
    {
        Error = error;
        Rule = rule;
    }

    /// <summary>
    /// Gets the Exception that was encountered when evaluation the rule.
    /// </summary>
    public Exception Error { get; }

    /// <summary>
    /// Gets the rule instance that encountered the exception when evaluation.
    /// </summary>
    public ISecurityRule Rule { get; }

    /// <summary>
    /// Returns a descriptive message describing the rule.
    /// </summary>
    /// <returns>String descibing the rule.</returns>
    public string BuildErrorMessage()
    {
        return $"{Rule.Description}/Error";
    }
}
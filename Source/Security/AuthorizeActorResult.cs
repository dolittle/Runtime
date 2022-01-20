// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Represents the result of an authorization of a <see cref="ISecurityActor"/>.
/// </summary>
public class AuthorizeActorResult
{
    readonly List<ISecurityRule> _brokenRules = new();
    readonly List<RuleEvaluationError> _rulesThatCausedError = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeActorResult"/> class.
    /// </summary>
    /// <param name="actorThatResultIsFor">The <see cref="ISecurityActor"/> that this <see cref="AuthorizeActorResult"/> pertains to.</param>
    public AuthorizeActorResult(ISecurityActor actorThatResultIsFor)
    {
        Actor = actorThatResultIsFor;
    }

    /// <summary>
    /// Gets the <see cref="ISecurityActor"/> that this <see cref="AuthorizeActorResult"/> pertains to.
    /// </summary>
    public ISecurityActor Actor { get; }

    /// <summary>
    /// Gets any <see cref="ISecurityRule"/> that were broken in the Authorization attempt.
    /// </summary>
    public IEnumerable<ISecurityRule> BrokenRules => _brokenRules.AsEnumerable();

    /// <summary>
    /// Gets any <see cref="RuleEvaluationError"/> that were encountered in the Authorization attempt.
    /// </summary>
    public IEnumerable<RuleEvaluationError> RulesThatEncounteredAnErrorWhenEvaluating => _rulesThatCausedError.AsEnumerable();

    /// <summary>
    /// Gets a value indicating whether indicates the Authorization attempt was successful or not.
    /// </summary>
    public virtual bool IsAuthorized => !RulesThatEncounteredAnErrorWhenEvaluating.Any() && !BrokenRules.Any();

    /// <summary>
    /// Add an instance of an <see cref="ISecurityRule"/> that was broken during Authorization.
    /// </summary>
    /// <param name="rule">An instance of a broken <see cref="ISecurityRule"/>.</param>
    public void AddBrokenRule(ISecurityRule rule)
    {
        _brokenRules.Add(rule);
    }

    /// <summary>
    /// Add an instance of an <see cref="ISecurityRule"/> that was unable to be evaluted because it encountered an exception.
    /// </summary>
    /// <param name="rule">The instance of the <see cref="ISecurityRule"/> that could not be evaluted.</param>
    /// <param name="exception">The exception that prevented the <see cref="ISecurityRule"/> from being evaluated.</param>
    public void AddErrorRule(ISecurityRule rule, Exception exception)
    {
        _rulesThatCausedError.Add(new RuleEvaluationError(rule, exception));
    }

    /// <summary>
    /// Builds a collection of strings that show Actor/Rule for each broken or erroring rule <see cref="AuthorizeActorResult"/>.
    /// </summary>
    /// <returns>A collection of strings.</returns>
    public virtual IEnumerable<string> BuildFailedAuthorizationMessages()
    {
        foreach (var brokenRule in BrokenRules)
            yield return Actor.Description + "/" + brokenRule.Description;

        foreach (var errorRule in RulesThatEncounteredAnErrorWhenEvaluating)
            yield return Actor.Description + "/" + errorRule.BuildErrorMessage();
    }
}
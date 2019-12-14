/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Runtime.Transactions;
using Dolittle.Runtime.Events;
using System.Collections.Generic;
using Dolittle.Rules;
using System.Linq.Expressions;
using System;

namespace Dolittle.Events
{
    /// <summary>
    /// An EventSource is a domain object that is capable of generating and applying events.  It is an AggregateRoot in the context
    /// of event sourcing.
    /// </summary>
    public interface IEventSource : ITransaction
    {
		/// <summary>
		/// The Id of the Event Source.  
		/// </summary>
		EventSourceId EventSourceId { get; }

		/// <summary>
		/// Gets the version of this EventSource
		/// </summary>
		EventSourceVersion Version { get; }

		/// <summary>
		/// Gets a stream of events that have been applied to the <seealso cref="EventSource">EventSource</seealso> but have not yet been committed to the EventStore.
		/// </summary>
		UncommittedEvents UncommittedEvents { get; }

		/// <summary>
		/// Gets a <see cref="IEnumerable{T}">collection</see> of <see cref="BrokenRule">broken rules</see>
		/// </summary>
		IEnumerable<BrokenRule> BrokenRules { get; }

		/// <summary>
		/// Gets a <see cref="IEnumerable{T}">collection</see> of <see cref="RuleSetEvaluation">evaluations</see>
		/// </summary>
		IEnumerable<RuleSetEvaluation> RuleSetEvaluations { get; }

		/// <summary>
		/// Builds an evaluation of rules that needs to be passed
		/// </summary>
		/// <param name="rules"><see cref="IRule">Rules</see> to evaluate</param>
		/// <returns><see cref="RuleSetEvaluation"/> for handling the evaluation</returns>
		RuleSetEvaluation Evaluate(params IRule[] rules);

		/// <summary>
		/// Builds an evaluation of rules that needs to be passed
		/// </summary>
		/// <param name="rules">Rules based on method expressions to evaluate</param>
		/// <returns><see cref="RuleSetEvaluation"/> for handling the evaluation</returns>
		RuleSetEvaluation Evaluate(params Expression<Func<RuleEvaluationResult>>[] rules);

		/// <summary>
		/// Apply a new event to the EventSource.  This will be applied and added to the <see cref="UncommittedEvents">UncommitedEvents</see>.
		/// </summary>
		/// <param name="event">The event that is to be applied</param>
    	void Apply(IEvent @event);

		/// <summary>
		/// Reapply an event from a stream
		/// </summary>
		/// <param name="eventStream">Stream that contains the events to reapply</param>
    	void ReApply(CommittedEvents eventStream);

        /// <summary>
        /// Fast forward to the specified version of the <seealso cref="EventSource">EventSource</seealso>
        /// </summary>
        /// <param name="version">Version to fast foward to</param>
        void FastForward(EventSourceVersion version);
    }
}
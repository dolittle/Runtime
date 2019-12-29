// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.Reflection;
using Dolittle.Rules;
using Dolittle.Runtime.Events;

namespace Dolittle.Events
{
    /// <summary>
    /// Represents a <see cref="IEventSource">IEventSource</see>.
    /// </summary>
    public class EventSource : IEventSource
    {
        readonly List<RuleSetEvaluation> _ruleSetEvaluations = new List<RuleSetEvaluation>();
        readonly List<BrokenRule> _brokenRules = new List<BrokenRule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSource"/> class.
        /// </summary>
        /// <param name="id"><see cref="EventSourceId"/> of the event source.</param>
        protected EventSource(EventSourceId id)
        {
            EventSourceId = id;
            UncommittedEvents = new UncommittedEvents(this);
            Version = EventSourceVersion.Initial;
        }

        /// <inheritdoc/>
        public EventSourceId EventSourceId { get; }

        /// <inheritdoc/>
        public EventSourceVersion Version { get; private set; }

        /// <inheritdoc/>
        public UncommittedEvents UncommittedEvents { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<BrokenRule> BrokenRules => _brokenRules;

        /// <inheritdoc/>
        public IEnumerable<RuleSetEvaluation> RuleSetEvaluations => _ruleSetEvaluations;

        /// <inheritdoc/>
        public RuleSetEvaluation Evaluate(params IRule[] rules)
        {
            var evaluation = new RuleSetEvaluation(new RuleSet(rules));
            evaluation.Evaluate(this);
            _ruleSetEvaluations.Add(evaluation);
            _brokenRules.AddRange(evaluation.BrokenRules);
            return evaluation;
        }

        /// <inheritdoc/>
        public RuleSetEvaluation Evaluate(params Expression<Func<RuleEvaluationResult>>[] rules)
        {
            var actualRules = new List<MethodRule>();
            foreach (var rule in rules)
            {
                var name = rule.GetMethodInfo().Name;
                var method = rule.Compile();
                actualRules.Add(new MethodRule(name, method));
            }

            return Evaluate(actualRules.ToArray());
        }

        /// <inheritdoc/>
        public void Apply(IEvent @event)
        {
            UncommittedEvents.Append(@event, Version);
            Version = Version.NextSequence();
            InvokeOnMethod(@event);
        }

        /// <inheritdoc/>
        public virtual void ReApply(CommittedEvents eventStream)
        {
            ValidateEventStream(eventStream);

            foreach (var committedEvent in eventStream)
            {
                InvokeOnMethod(committedEvent.Event);
                Version = committedEvent.Version.ToEventSourceVersion();
            }

            Version = Version.NextCommit();
        }

        /// <inheritdoc/>
        public void FastForward(EventSourceVersion lastVersion)
        {
            ThrowIfStateful();
            ThrowIfNotInitialVersion();

            Version = lastVersion == null ? EventSourceVersion.Initial : lastVersion.NextCommit();
        }

        /// <inheritdoc/>
        public virtual void Commit()
        {
            UncommittedEvents = new UncommittedEvents(this);
            Version = Version.NextCommit();
        }

        /// <inheritdoc/>
        public virtual void Rollback()
        {
            UncommittedEvents = new UncommittedEvents(this);
            Version = Version.PreviousCommit();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Commit();
        }

        void InvokeOnMethod(IEvent @event)
        {
            var handleMethod = this.GetOnMethod(@event);
            handleMethod?.Invoke(this, new[] { @event });
        }

        void ValidateEventStream(CommittedEvents eventStream)
        {
            if (!IsForThisEventSource(eventStream.EventSourceId))
            {
                throw new EventBelongsToOtherEventSource(eventStream.EventSourceId, EventSourceId);
            }
        }

        bool IsForThisEventSource(Guid targetEventSourceId)
        {
            return targetEventSourceId == EventSourceId;
        }

        void ThrowIfStateful()
        {
            if (!this.IsStateless())
                throw new FastForwardNoAllowedForStatefulEventSource(GetType());
        }

        void ThrowIfNotInitialVersion()
        {
            if (!Version.Equals(EventSourceVersion.Initial))
                throw new FastForwardNotAllowedForStatefulEventSourceWithEvents(GetType());
        }
    }
}

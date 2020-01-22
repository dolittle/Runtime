// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Types;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessors"/>.
    /// </summary>
    [Singleton]
    public class EventProcessors : IEventProcessors
    {
        readonly IInstancesOf<IKnowAboutEventProcessors> _systemsThatKnowsAboutEventProcessors;
        readonly List<IEventProcessor> _eventProcessors = new List<IEventProcessor>();
        readonly ILogger _logger;
        readonly IObjectFactory _objectFactory;
        Dictionary<Artifact, List<IEventProcessor>> _eventProcessorsByResourceIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessors"/> class.
        /// </summary>
        /// <param name="systemsThatKnowsAboutEventProcessors">Instances of <see cref="IKnowAboutEventProcessors"/>.</param>
        /// <param name="objectFactory"><see cref="IObjectFactory"/> for creating instances of objects.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventProcessors(
            IInstancesOf<IKnowAboutEventProcessors> systemsThatKnowsAboutEventProcessors,
            IObjectFactory objectFactory,
            ILogger logger)
        {
            _systemsThatKnowsAboutEventProcessors = systemsThatKnowsAboutEventProcessors;
            GatherEventProcessors();
            _logger = logger;
            _objectFactory = objectFactory;
        }

        /// <inheritdoc/>
        public IEnumerable<IEventProcessor> All => _eventProcessors;

        /// <summary>
        /// Process a <see cref="Store.CommittedEventStream"/>.
        /// </summary>
        /// <param name="committedEvents"><see cref="Store.CommittedEventStream"/> to process.</param>
        public void Process(Store.CommittedEventStream committedEvents)
        {
            foreach (var eventEnvelope in committedEvents.Events)
            {
                if (!_eventProcessorsByResourceIdentifier.ContainsKey(eventEnvelope.Metadata.Artifact)) GatherEventProcessors();
                if (_eventProcessorsByResourceIdentifier.ContainsKey(eventEnvelope.Metadata.Artifact))
                {
                    var processors = _eventProcessorsByResourceIdentifier[eventEnvelope.Metadata.Artifact];
                    processors.ForEach(_ => _.Process(eventEnvelope.ToCommittedEventEnvelope(committedEvents.Sequence)));
                }
            }
        }

        /// <summary>
        /// Gather event processors in the system.
        /// </summary>
        public void GatherEventProcessors()
        {
            var eventProcessorsByArtifact = new Dictionary<Artifact, List<IEventProcessor>>();
            _systemsThatKnowsAboutEventProcessors.ForEach(a => a.ForEach(e =>
            {
                List<IEventProcessor> eventProcessors;
                if (eventProcessorsByArtifact.ContainsKey(e.Event))
                {
                    eventProcessors = eventProcessorsByArtifact[e.Event];
                }
                else
                {
                    eventProcessors = new List<IEventProcessor>();
                    eventProcessorsByArtifact[e.Event] = eventProcessors;
                }

                eventProcessors.Add(e);
            }));

            _eventProcessorsByResourceIdentifier = eventProcessorsByArtifact;
        }
    }
}
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.PropertyBags;
using Dolittle.Types;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessors"/>
    /// </summary>
    [Singleton]
    public class EventProcessors : IEventProcessors
    {
        readonly Dictionary<Artifact, List<IEventProcessor>> _eventProcessorsByResourceIdentifier;
        readonly List<IEventProcessor> _eventProcessors = new List<IEventProcessor>();
        readonly ILogger _logger;
        readonly IObjectFactory _objectFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="EventProcessors"/>
        /// </summary>
        /// <param name="systemsThatKnowsAboutEventProcessors">Instances of <see cref="IKnowAboutEventProcessors"/></param>
        /// <param name="objectFactory"></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>

        public EventProcessors(
            IInstancesOf<IKnowAboutEventProcessors> systemsThatKnowsAboutEventProcessors,
            IObjectFactory objectFactory,
            ILogger logger)
        {
            _eventProcessorsByResourceIdentifier = GatherEventProcessorsFrom(systemsThatKnowsAboutEventProcessors);
            _logger = logger;
            _objectFactory = objectFactory;
        }

        /// <inheritdoc/>
        public IEnumerable<IEventProcessor> All => _eventProcessors;

        /// <inheritdoc/>
        public void Process(Store.CommittedEventStream committedEvents)
        {
            foreach( var eventEnvelope in committedEvents.Events )
            {
                var processors = _eventProcessorsByResourceIdentifier[eventEnvelope.Metadata.Artifact];
                processors.ForEach(_ => _.Process(eventEnvelope));
            }
        }

        Dictionary<Artifact, List<IEventProcessor>> GatherEventProcessorsFrom(IEnumerable<IKnowAboutEventProcessors> systemsThatHasEventProcessors)
        {
            var eventProcessorsByArtifact = new Dictionary<Artifact, List<IEventProcessor>>();
            systemsThatHasEventProcessors.ForEach(a => a.ForEach(e =>
            {
                List<IEventProcessor> eventProcessors;
                if (eventProcessorsByArtifact.ContainsKey(e.Event)) eventProcessors = eventProcessorsByArtifact[e.Event];
                else
                {
                    eventProcessors = new List<IEventProcessor>();
                    eventProcessorsByArtifact[e.Event] = eventProcessors;
                }
                eventProcessors.Add(e);
            }));

            return eventProcessorsByArtifact;
        }
    }
}
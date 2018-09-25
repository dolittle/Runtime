/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using Dolittle.Collections;
using Dolittle.Concepts.Serialization.Json;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonsConfigurationManager"/>
    /// </summary>
    [Singleton]
    public class EventHorizonsConfigurationManager : IEventHorizonsConfigurationManager
    {
        const string _configurationFile = ".dolittle/event-horizons.json";
        readonly ILogger _logger;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of <see cref="EventHorizonsConfigurationManager"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use for dealing with configuration as JSON</param>
        /// <param name="logger"><see cref="ILogger"/> to use for logging</param>
        public EventHorizonsConfigurationManager(ISerializer serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
            if (File.Exists(_configurationFile))
            {
                logger.Information("Configuration file found for event horizons, loading it");
                Current = LoadConfig();
            }
            else
            { 
                logger.Information("No configuration file for event horizons found - no connections will be made");
                Current = new EventHorizonsConfiguration();
            }
        }

        /// <inheritdoc/>
        public EventHorizonsConfiguration Current { get; }


        EventHorizonsConfiguration LoadConfig()
        {
            var json = File.ReadAllText(_configurationFile);
            var current = _serializer.FromJson<EventHorizonsConfiguration>(json);
            current.EventHorizons.ForEach(eventHorizonConfig =>
            {
                _logger.Information($"This singularity will be connected to event horizon at '{eventHorizonConfig.Url}' for application '{eventHorizonConfig.Application}' and bounded context '{eventHorizonConfig.BoundedContext}'");
            });

            return current;
        }
    }
}
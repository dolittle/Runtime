/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using Dolittle.Collections;
using Dolittle.Concepts.Serialization.Json;
using Dolittle.Execution;
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
        const string _configurationFile = "event-horizons.json";
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="EventHorizonsConfigurationManager"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> to use for logging</param>
        public EventHorizonsConfigurationManager(ILogger logger)
        {
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
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            serializerSettings.Converters.Add(new ConceptConverter());
            serializerSettings.Converters.Add(new ConceptDictionaryConverter());

            var json = File.ReadAllText(_configurationFile);
            var current = JsonConvert.DeserializeObject<EventHorizonsConfiguration>(json, serializerSettings);
            _logger.Information($"EventHorizon will be exposed on TCP with port {current.Port}");
            _logger.Information($"EventHorizon will be exposed on Unix socket {current.UnixSocket}");
            current.EventHorizons.ForEach(eventHorizonConfig =>
            {
                _logger.Information($"This singularity will be connected to event horizon at '{eventHorizonConfig.Url}' for application '{eventHorizonConfig.Application}' and bounded context '{eventHorizonConfig.BoundedContext}'");
            });

            return current;
        }

    }
}
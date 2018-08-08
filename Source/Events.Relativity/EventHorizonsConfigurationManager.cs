/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Serialization.Json;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonsConfigurationManager"/>
    /// </summary>
    [Singleton]
    public class EventHorizonsConfigurationManager : IEventHorizonsConfigurationManager
    {
        const string _configurationFile = "event-horizons.json";

        /// <summary>
        /// Initializes a new instance of <see cref="EventHorizonsConfigurationManager"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use for parsing configuration</param>
        /// <param name="logger"><see cref="ILogger"/> to use for logging</param>
        public EventHorizonsConfigurationManager(ISerializer serializer, ILogger logger)
        {
            if (File.Exists(_configurationFile))
            {
                logger.Information("Configuration file found for event horizons, loading it");
                Current = serializer.FromJson<EventHorizonsConfiguration>(File.ReadAllText(_configurationFile));
            }
            else
            { 
                logger.Information("No configuration file for event horizons found - no connections will be made");
                Current = new EventHorizonsConfiguration();
            }
        }

        /// <inheritdoc/>
        public EventHorizonsConfiguration Current { get; }
    }
}
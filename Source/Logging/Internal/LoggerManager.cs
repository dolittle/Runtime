// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Logging.Booting;

namespace Dolittle.Runtime.Logging.Internal
{
    /// <summary>
    /// An implementation of <see cref="ILoggerManager"/>.
    /// </summary>
    [Singleton]
    public class LoggerManager : ILoggerManager
    {
        readonly IDictionary<Type, InternalLogger> _loggers;
        bool _isCapturingBootLogs;
        ILogMessageWriterCreator[] _creators;

        LoggerManager()
        {
            _loggers = new Dictionary<Type, InternalLogger>();
            _isCapturingBootLogs = true;
            _creators = new ILogMessageWriterCreator[] { new BootLogMessageWriterCreator() };
        }

        /// <summary>
        /// Gets the static singleton instance of the <see cref="LoggerManager"/>.
        /// </summary>
        public static ILoggerManager Instance { get; } = new LoggerManager();

        /// <inheritdoc/>
        public void AddLogMessageWriterCreators(params ILogMessageWriterCreator[] creators)
        {
            lock (_loggers)
            {
                if (!_isCapturingBootLogs)
                {
                    _creators = _creators.Union(creators).ToArray();
                    foreach ((var type, var logger) in _loggers)
                    {
                        logger.LogMessageWriters = CreateWriters(type);
                    }
                }
                else
                {
                    var bootCreator = _creators[0] as BootLogMessageWriterCreator;
                    _creators = creators;

                    var newLogWriters = new Dictionary<Type, ILogMessageWriter[]>();

                    ILogMessageWriter[] WritersForType(Type type)
                    {
                        if (!newLogWriters.TryGetValue(type, out var writers))
                        {
                            writers = CreateWriters(type);
                            newLogWriters[type] = writers;
                        }

                        return writers;
                    }

                    bootCreator.FlushTo(WritersForType);

                    foreach ((var type, var logger) in _loggers)
                    {
                        logger.LogMessageWriters = WritersForType(type);
                    }

                    bootCreator.FlushTo(WritersForType);

                    _isCapturingBootLogs = false;
                }
            }
        }

        /// <inheritdoc/>
        public ILogger<T> CreateLogger<T>() => CreateLogger(typeof(T)) as ILogger<T>;

        /// <inheritdoc/>
        public ILogger CreateLogger(Type type)
        {
            lock (_loggers)
            {
                if (!_loggers.TryGetValue(type, out var logger))
                {
                    if (type == typeof(UnknownLogMessageSource))
                        logger = new UnknownLogger();
                    else
                        logger = Activator.CreateInstance(typeof(InternalLogger<>).MakeGenericType(type)) as InternalLogger;

                    logger.LogMessageWriters = CreateWriters(type);

                    _loggers[type] = logger;
                }

                return logger;
            }
        }

        ILogMessageWriter[] CreateWriters(Type type)
        {
            var writers = new ILogMessageWriter[_creators.Length];

            for (var i = 0; i < _creators.Length; ++i)
            {
                writers[i] = _creators[i].CreateFor(type);
            }

            return writers;
        }
    }
}

/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Applications;
using Dolittle.Logging;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Storage.Development
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventSourceVersions"/> for holding <see cref="EventSourceVersion"/>
    /// for each <see cref="EventSource"/> on the filesystem
    /// </summary>
    public class EventSourceVersions : IEventSourceVersions
    {
        const string VersionForPrefix = "VersionFor";

        IFiles _files;
        IApplicationArtifactIdentifierStringConverter _applicationArtifactIdentifierStringConverter;
        IEventStore _eventStore;
        string _path;

        /// <summary>
        /// Initializes a new instance of <see cref="EventSourceVersions"/>
        /// </summary>
        /// <param name="files">A system to work with <see cref="IFiles"/></param>
        /// <param name="eventStore"><see cref="IEventStore"/> for getting information if not found in file system</param>
        /// <param name="applicationArtifactIdentifierStringConverter">Converter for converting <see cref="IApplicationArtifactIdentifier"/> "/></param>
        /// <param name="pathProvider">A delegate that can provide path to store <see cref="EventSourceVersion"/> for <see cref="IEventSource"/> - see <see cref="ICanProvideEventSourceVersionsPath"/></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public EventSourceVersions(
            IFiles files, 
            IEventStore eventStore, 
            IApplicationArtifactIdentifierStringConverter applicationArtifactIdentifierStringConverter, 
            ICanProvideEventSourceVersionsPath pathProvider,
            ILogger logger)
        {
            _files = files;
            _eventStore = eventStore;
            _applicationArtifactIdentifierStringConverter = applicationArtifactIdentifierStringConverter;
            _path = pathProvider();
            logger.Information($"Using path : {_path}");
        }

        /// <inheritdoc/>
        public EventSourceVersion GetFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId)
        {
            var fileName = GetFileNameFor(eventSource, eventSourceId);
            var version = EventSourceVersion.Zero;

            if( _files.Exists(_path, fileName) )
            {
                var versionAsString = _files.ReadString(_path, fileName);
                version = EventSourceVersion.FromCombined(double.Parse(versionAsString));
            }  else version = _eventStore.GetVersionFor(eventSource, eventSourceId);

            return version;
        }

        /// <inheritdoc/>
        public void SetFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId, EventSourceVersion version)
        {
            var fileName = GetFileNameFor(eventSource, eventSourceId);
            _files.WriteString(_path, fileName, version.Combine().ToString());
        }


        string GetFileNameFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId)
        {
            var key = $"{VersionForPrefix}_{_applicationArtifactIdentifierStringConverter.AsString(eventSource)}_{eventSourceId.Value}";
            return key;
        }
    }
}

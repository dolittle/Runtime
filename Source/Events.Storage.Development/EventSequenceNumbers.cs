/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using doLittle.Applications;
using doLittle.Execution;
using doLittle.Logging;

namespace doLittle.Runtime.Events.Storage.Development
{
    /// <summary>
    /// Represents a simple and naïve implementation of <see cref="IEventSequenceNumbers"/>
    /// </summary>
    [Singleton]
    public class EventSequenceNumbers : IEventSequenceNumbers
    {
        const string SequenceFileName = "sequence";
        const string SequenceForPrefix = "sequence_for_";

        object _globalSequenceLock = new object();
        Dictionary<int, object> _sequenceLocksPerType = new Dictionary<int, object>();
        IApplicationArtifactIdentifierStringConverter _applicationArtifactIdentifierStringConverter;
        IFiles _files;
        string _path;

        /// <summary>
        /// Initializes a new instance of <see cref="EventSequenceNumbers"/>
        /// </summary>
        /// <param name="applicationArtifactIdentifierStringConverter"><see cref="IApplicationArtifactIdentifierStringConverter"/> for getting string representation of <see cref="IApplicationArtifactIdentifier"/></param>
        /// <param name="files"><see cref="IFiles"/> to work with files</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public EventSequenceNumbers(
            IApplicationArtifactIdentifierStringConverter applicationArtifactIdentifierStringConverter, 
            IFiles files,
            ILogger logger)
        {
            _applicationArtifactIdentifierStringConverter = applicationArtifactIdentifierStringConverter;
            _files = files;
            _path = "";
            throw new NotImplementedException("MISSING PATH CONFIGURATION");
        }


        /// <inheritdoc/>
        public EventSequenceNumber Next()
        {
            lock( _globalSequenceLock )
            {
                var sequence = GetNextInSequenceFromFile(SequenceFileName);
                _files.WriteString(_path, SequenceFileName, sequence.ToString());
                return sequence;
            }
        }

        /// <inheritdoc/>
        public EventSequenceNumber NextForType(IApplicationArtifactIdentifier identifier)
        {
            var hashCode = identifier.GetHashCode();
            lock( _sequenceLocksPerType )
            {
                if (!_sequenceLocksPerType.ContainsKey(hashCode)) _sequenceLocksPerType[hashCode] = new object();
            }

            lock( _sequenceLocksPerType[hashCode] )
            {
                var identifierAsString = _applicationArtifactIdentifierStringConverter.AsString(identifier);
                var file = $"{SequenceForPrefix}{identifierAsString}";
                var sequence = GetNextInSequenceFromFile(file);
                _files.WriteString(_path, file, sequence.ToString());
                return sequence;
            }
        }

        long GetNextInSequenceFromFile(string file)
        {
            var sequence = 0L;
            if( _files.Exists(_path, file)) sequence = long.Parse(_files.ReadString(_path, file));
            sequence++;
            _files.WriteString(_path, file, sequence.ToString());
            return sequence;
        }
    }
}

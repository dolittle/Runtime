// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents registrations of <see cref="StreamProcessor" /> as an unit in a transaction.
    /// </summary>
    public class StreamProcessorRegistrations : IDisposable
    {
        readonly IList<StreamProcessorRegistration> _registrations;
        bool _disposed;
        bool _started;

        /// <summary>
        /// Gets a value indicating whether any of the <see cref="StreamProcessorRegistration" />s failed.
        /// </summary>
        /// <returns>A value indicating whether any of the <see cref="StreamProcessorRegistration" />s failed.</returns>
        public bool HasFailures => _registrations.Any(_ => !_.Succeeded);

        /// <summary>
        /// Add a <see cref="StreamProcessorRegistration" />.
        /// </summary>
        /// <param name="streamProcessorRegistration">The <see cref="StreamProcessorRegistration" />.</param>
        public void Add(StreamProcessorRegistration streamProcessorRegistration) => _registrations.Add(streamProcessorRegistration);

        /// <summary>
        /// Try to <see cref="StreamProcessor.Start" /> all <see cref="StreamProcessor" />s.
        /// </summary>
        /// <returns>A value indicating whether all registered <see cref="StreamProcessor" />s could be started.</returns>
        public bool TryStart()
        {
            if (_started || _disposed) return false;
            _started = true;
            var succeededRegistrations = _registrations.Where(_ => _.Succeeded);
            if (succeededRegistrations.Count() != _registrations.Count) return false;
            succeededRegistrations.Select(_ => _.StreamProcessor).ForEach(_ => _.Start());

            return true;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _registrations.ForEach(_ => _.Dispose());
            }

            _disposed = true;
        }
    }
}

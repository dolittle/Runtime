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
    public class StreamProcessorRegistrations : List<StreamProcessorRegistration>, IDisposable
    {
        bool _disposed;
        bool _started;

        /// <summary>
        /// Gets a value indicating whether any of the <see cref="StreamProcessorRegistration" />s failed.
        /// </summary>
        /// <returns>A value indicating whether any of the <see cref="StreamProcessorRegistration" />s failed.</returns>
        public bool HasFailures => this.Any(_ => !_.Succeeded);

        /// <summary>
        /// Try to <see cref="StreamProcessor.Start" /> all <see cref="StreamProcessor" />s.
        /// </summary>
        /// <returns>A value indicating whether all registered <see cref="StreamProcessor" />s could be started.</returns>
        public bool TryStart()
        {
            if (_started || _disposed) return false;
            _started = true;
            if (HasFailures) return false;
            foreach (var registration in this)
            {
                registration.StreamProcessor.Start();
            }

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
                ForEach(_ => _.Dispose());
            }

            _disposed = true;
        }
    }
}

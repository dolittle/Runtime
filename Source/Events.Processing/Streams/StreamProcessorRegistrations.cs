// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents the lifecycle of stream processors.
    /// </summary>
    public class StreamProcessorRegistrations : IDisposable
    {
        readonly IList<(TenantId, StreamProcessorRegistrationResult)> _registrationResults;

        bool _disposed;

        /// <summary>
        /// Add a <see cref="StreamProcessorRegistrationResult" />.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <param name="streamProcessorRegistrationResult">The <see cref="StreamProcessorRegistrationResult" />.</param>
        public void Add(TenantId tenant, StreamProcessorRegistrationResult streamProcessorRegistrationResult)
        {
            _registrationResults.Add((tenant, streamProcessorRegistrationResult));
        }

        /// <summary>
        /// Whether any of the <see cref="StreamProcessorRegistrationResult" />s failed.
        /// </summary>
        /// <returns>A value indicating whether any of the <see cref="StreamProcessorRegistrationResult" />s failed.</returns>
        public bool HasFailures() => _registrationResults.Any(_ => !_.Item2.Succeeded);

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
                _registrationResults
                    .Select(_ => _.Item2)
                    .Where(_ => _.Succeeded)
                    .ForEach(_ => _.StreamProcessor.Dispose());
            }

            _disposed = true;
        }
    }
}
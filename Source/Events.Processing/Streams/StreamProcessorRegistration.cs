// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents the registration of a stream processor.
    /// </summary>
    public abstract class StreamProcessorRegistration : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorRegistration"/> class.
        /// </summary>
        /// <param name="streamProcessor">The <see cref="StreamProcessor" />.</param>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        protected StreamProcessorRegistration(StreamProcessor streamProcessor, TenantId tenant)
        {
            Succeeded = true;
            Tenant = tenant;
            StreamProcessor = streamProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorRegistration"/> class.
        /// </summary>
        /// <param name="failureReason">The <see cref="StreamProcessorRegistrationFailureReason" />.</param>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        protected StreamProcessorRegistration(StreamProcessorRegistrationFailureReason failureReason, TenantId tenant)
        {
            Succeeded = false;
            Tenant = tenant;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether a new <see cref="StreamProcessor" /> was registered.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the <see cref="TenantId" /> that the registration happened for.
        /// </summary>
        public TenantId Tenant { get; }

        /// <summary>
        /// Gets the <see cref="StreamProcessorRegistrationFailureReason" />.
        /// </summary>
        public StreamProcessorRegistrationFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the registered stream processor.
        /// </summary>
        public StreamProcessor StreamProcessor { get; }

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            Disposed = true;
        }
    }
}

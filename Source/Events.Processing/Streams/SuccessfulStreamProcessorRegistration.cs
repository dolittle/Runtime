// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a successful <see cref="StreamProcessorRegistration" />.
    /// </summary>
    public class SuccessfulStreamProcessorRegistration : StreamProcessorRegistration
    {
        readonly Action _unregister;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulStreamProcessorRegistration"/> class.
        /// </summary>
        /// <param name="streamProcessor">The successfully <see cref="StreamProcessor" />.</param>
        /// <param name="tenant"><see cref="TenantId" />.</param>
        /// <param name="unregister">The <see cref="Action" /> that unregisters the <see cref="StreamProcessor" />.</param>
        public SuccessfulStreamProcessorRegistration(StreamProcessor streamProcessor, TenantId tenant, Action unregister)
            : base(streamProcessor, tenant)
        {
            _unregister = unregister;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (Disposed) return;
            base.Dispose(disposing);
            _unregister();
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a successful <see cref="StreamProcessorRegistration" />.
    /// </summary>
    public class SuccessfulStreamProcessorRegistration : StreamProcessorRegistration
    {
        readonly CancellationTokenRegistration _unregisterOnCancellationRegistration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulStreamProcessorRegistration"/> class.
        /// </summary>
        /// <param name="streamProcessor">The successfully <see cref="StreamProcessor" />.</param>
        /// <param name="tenant"><see cref="TenantId" />.</param>
        /// <param name="unregisterOnCancellationRegistration">The <see cref="CancellationTokenRegistration" /> that registered the action to unregister the <see cref="StreamProcessor" /> when <see cref="CancellationToken" /> is cancelled.</param>
        public SuccessfulStreamProcessorRegistration(StreamProcessor streamProcessor, TenantId tenant, CancellationTokenRegistration unregisterOnCancellationRegistration)
            : base(streamProcessor, tenant)
        {
            _unregisterOnCancellationRegistration = unregisterOnCancellationRegistration;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (Disposed) return;
            base.Dispose(disposing);
            _unregisterOnCancellationRegistration.Dispose();
        }
    }
}

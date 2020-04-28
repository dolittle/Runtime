// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a successful <see cref="StreamProcessorRegistration" />.
    /// </summary>
    public class SuccessfulStreamProcessorRegistration : StreamProcessorRegistration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulStreamProcessorRegistration"/> class.
        /// </summary>
        /// <param name="streamProcessor">The successfully <see cref="AbstractStreamProcessor" />.</param>
        /// <param name="tenant"><see cref="TenantId" />.</param>
        public SuccessfulStreamProcessorRegistration(AbstractStreamProcessor streamProcessor, TenantId tenant)
            : base(streamProcessor, tenant)
        {
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (Disposed) return;
            base.Dispose(disposing);
        }
    }
}

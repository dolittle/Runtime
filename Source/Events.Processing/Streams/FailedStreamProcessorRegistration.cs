// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a failed <see cref="StreamProcessorRegistration" />.
    /// </summary>
    public class FailedStreamProcessorRegistration : StreamProcessorRegistration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedStreamProcessorRegistration"/> class.
        /// </summary>
        /// <param name="reason">The <see cref="StreamProcessorRegistrationFailureReason" />.</param>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        public FailedStreamProcessorRegistration(StreamProcessorRegistrationFailureReason reason, TenantId tenant)
            : base(reason, tenant)
        {
        }
    }
}

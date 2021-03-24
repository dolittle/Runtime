// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Projections.
    /// </summary>
    public static class ProjectionFailures
    {
        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'NoProjectionRegistrationReceived' failure type.
        /// </summary>
        public static FailureId NoProjectionRegistrationReceived => FailureId.Create("1547a083-6a6e-4c49-886d-6852c029b252");

        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'FailedToRegisterProjection' failure type.
        /// </summary>
        public static FailureId FailedToRegisterProjection => FailureId.Create("eb82f4d3-06e5-45cc-ac97-7d77dd1e5d07");
    }
}

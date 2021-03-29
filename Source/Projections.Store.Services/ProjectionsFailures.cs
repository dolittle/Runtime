// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Projections.Store.Services
{
    /// <summary>
    /// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Projections.
    /// </summary>
    public static class ProjectionsFailures
    {
        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'FailedToGetProjectionDefinition' failure type.
        /// </summary>
        public static readonly FailureId FailedToGetProjectionDefinition = FailureId.Create("0cbd47f6-50b4-4488-afc3-6ba6b02b0dbd");
    }
}

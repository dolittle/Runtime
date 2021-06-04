// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a system that uniquely identifies service requests by assigning an unique <see cref="RequestId"/> to each request.
    /// </summary>
    public interface IIdentifyRequests
    {
        /// <summary>
        /// Get the <see cref="RequestId"/> identifying a request from the corresponding <see cref="ServerCallContext"/>.
        /// </summary>
        /// <param name="callContext">The <see cref="ServerCallContext"/> of the request to identify.</param>
        /// <returns>An identifier that identifies the request.</returns>
        RequestId GetRequestIdFor(ServerCallContext callContext);
    }
}

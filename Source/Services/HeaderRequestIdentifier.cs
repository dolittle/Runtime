// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IIdentifyRequests"/> that looks for an 'X-Request-ID' HTTP header, or generates a new random request id.
    /// </summary>
    public class HeaderRequestIdentifier : IIdentifyRequests
    {
        const string X_REQUEST_ID_HEADER_LOWERCASE = "x-request-id";

        /// <inheritdoc/>
        public RequestId GetRequestIdFor(ServerCallContext callContext)
        {
            foreach (var header in callContext.RequestHeaders)
            {
                if (!header.IsBinary && header.Key.ToLowerInvariant() == X_REQUEST_ID_HEADER_LOWERCASE)
                {
                    return header.Value;
                }
            }

            return RequestId.Generate();
        }
    }
}

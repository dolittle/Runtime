// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Handshake.Contracts.Handshake;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Runtime.Server.Handshake
{
    /// <summary>
    /// Represents the implementation of.
    /// </summary>
    public class HandshakeService : HandshakeBase
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakeService"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public HandshakeService(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public override async Task<HandshakeResponse> Handshake(HandshakeRequest request, ServerCallContext context)
        {
            try
            {
                return new HandshakeResponse
                {
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning("");
                return new HandshakeResponse
                {
                    Failure = new Failure
                    {
                        Id = FailureId.Other.ToProtobuf(),
                        Reason = ex.Message
                    }
                };
            }
        }

        // [LoggerMessage()]
        // partial void Log();
    }
}

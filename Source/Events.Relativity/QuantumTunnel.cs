/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IQuantumTunnel"/>
    /// </summary>
    public class QuantumTunnel : IQuantumTunnel
    {
        readonly IServerStreamWriter<EventParticleMessage> _responseStream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseStream"></param>
        public QuantumTunnel(IServerStreamWriter<EventParticleMessage> responseStream)
        {
            _responseStream = responseStream;
        }
    }
}
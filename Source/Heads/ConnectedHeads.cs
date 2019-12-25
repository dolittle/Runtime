// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents an implementation of <see cref="IConnectedHeads"/>.
    /// </summary>
    [Singleton]
    public class ConnectedHeads : IConnectedHeads
    {
        readonly List<Head> _heads = new List<Head>();
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedHeads"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ConnectedHeads(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Connect(Head head)
        {
            lock (_heads) _heads.Add(head);
        }

        /// <inheritdoc/>
        public void Disconnect(HeadId headId)
        {
            lock (_heads)
            {
                var head = _heads.SingleOrDefault(_ => _.HeadId == headId);
                if (head != null)
                {
                    _logger.Information($"Disconnecting head '{headId}'");
                    _heads?.Remove(head);
                }
            }
        }

        /// <inheritdoc/>
        public bool IsConnected(HeadId headId)
        {
            lock (_heads)
            {
                return _heads.Any(_ => _.HeadId == headId);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Head> GetAll()
        {
            lock (_heads)
            {
                var heads = _heads.ToArray();
                return heads;
            }
        }

        /// <inheritdoc/>
        public Head GetFor<TC>()
            where TC : ClientBase
        {
            return _heads.Last();
        }

        /// <inheritdoc/>
        public Head GetFor(Type type)
        {
            return _heads.Last();
        }

        /// <inheritdoc/>
        public Head GetById(HeadId headId)
        {
            return _heads.SingleOrDefault(_ => _.HeadId == headId);
        }
    }
}
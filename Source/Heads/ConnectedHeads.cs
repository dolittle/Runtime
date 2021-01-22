// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents an implementation of <see cref="IConnectedHeads"/>.
    /// </summary>
    [Singleton]
    public class ConnectedHeads : IConnectedHeads
    {
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
        public ObservableCollection<Head> All { get; } = new ObservableCollection<Head>();

        /// <inheritdoc/>
        public void Connect(Head head)
        {
            lock (All) All.Add(head);
        }

        /// <inheritdoc/>
        public void Disconnect(HeadId headId)
        {
            lock (All)
            {
                var head = All.SingleOrDefault(_ => _.HeadId == headId);
                if (head != null)
                {
                    _logger.Debug("Disconnecting head '{HeadId}'", headId);
                    All.Remove(head);
                }
            }
        }

        /// <inheritdoc/>
        public bool IsConnected(HeadId headId)
        {
            lock (All)
            {
                return All.Any(_ => _.HeadId == headId);
            }
        }

        /// <inheritdoc/>
        public Head GetById(HeadId headId)
        {
            lock (All)
            {
                return All.SingleOrDefault(_ => _.HeadId == headId);
            }
        }
    }
}
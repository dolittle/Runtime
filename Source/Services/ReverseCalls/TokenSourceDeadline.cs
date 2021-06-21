// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Represents an implementation of <see cref="ICancelTokenIfDeadlineIsMissed"/> that uses a <see cref="CancellationTokenSource"/>.
    /// </summary>
    public class TokenSourceDeadline : ICancelTokenIfDeadlineIsMissed
    {
        readonly CancellationTokenSource _source = new();
        bool _disposed;

        /// <inheritdoc/>
        public void RefreshDeadline(TimeSpan nextRefreshBefore)
        {
            if (nextRefreshBefore == TimeSpan.Zero)
            {
                _source.Cancel();
                return;
            }

            _source.CancelAfter(nextRefreshBefore);
        }

        /// <inheritdoc/>
        public CancellationToken Token => _source.Token;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _source.Dispose();
            }

            _disposed = true;
        }
    }
}

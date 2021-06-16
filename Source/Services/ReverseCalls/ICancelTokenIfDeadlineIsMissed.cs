// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Defines a system that cancels a <see cref="CancellationToken"/> if a deadline for refreshing the timeout is missed.
    /// </summary>
    /// <remarks>
    /// The deadline starts out as infinite, and the token will never be cancelled if the initial refresh is never called.
    /// Disposing of the system will leave the token in whatever state it is currently in.
    /// </remarks>
    public interface ICancelTokenIfDeadlineIsMissed : IDisposable
    {
        /// <summary>
        /// Refreshes the deadline for cancellation.
        /// </summary>
        /// <param name="nextRefreshBefore">The time to wait for a new refresh before cancelling the token.</param>
        /// <remarks>
        /// Refreshing with <see cref="TimeSpan.Zero"/> will cancel the token immediately.
        /// <remarks>
        void RefreshDeadline(TimeSpan nextRefreshBefore);

        /// <summary>
        /// Gets the token that will be cancelled if a refresh deadline is missed.
        /// </summary>
        CancellationToken Token { get; }
    }
}

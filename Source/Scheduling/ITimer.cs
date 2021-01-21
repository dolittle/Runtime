// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Scheduling
{
    /// <summary>
    /// Defines an instance of a timer.
    /// </summary>
    public interface ITimer : IDisposable
    {
        /// <summary>
        /// Stop a running timer.
        /// </summary>
        void Stop();
    }
}
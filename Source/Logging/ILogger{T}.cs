// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Logging
{
    /// <summary>
    /// Defines a system for writing log messages.
    /// </summary>
    /// <typeparam name="T">The type that the log messages relate to.</typeparam>
    public interface ILogger<T> : ILogger
    {
    }
}

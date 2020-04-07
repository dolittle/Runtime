// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Logging.Management
{
    /// <summary>
    /// Defines a system for managing logs for management purposes.
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Gets a <see cref="ObservableCollection{T}"/> of <see cref="string">log messages</see>.
        /// </summary>
        ObservableCollection<string> Messages { get; }

        /// <summary>
        /// Write a log message.
        /// </summary>
        /// <param name="message"><see cref="string"/> message to write.</param>
        void Write(string message);
    }
}
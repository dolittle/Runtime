// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.ObjectModel;
using Dolittle.Lifecycle;

namespace Dolittle.Runtime.Logging.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ILogManager"/>.
    /// </summary>
    [Singleton]
    public class LogManager : ILogManager
    {
        const int MaxNumberOfMessages = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        public LogManager()
        {
            Messages = new ObservableCollection<string>();
        }

        /// <inheritdoc/>
        public ObservableCollection<string> Messages { get; }

        /// <inheritdoc/>
        public void Write(string message)
        {
            Messages.Add(message);

            while (Messages.Count > MaxNumberOfMessages)
            {
                Messages.RemoveAt(0);
            }
        }
    }
}
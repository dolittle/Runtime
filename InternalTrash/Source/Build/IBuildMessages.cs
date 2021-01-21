// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Build
{
    /// <summary>
    /// Defines a system for outputting build messages.
    /// </summary>
    public interface IBuildMessages
    {
        /// <summary>
        /// Push an indentation.
        /// </summary>
        void Indent();

        /// <summary>
        /// Pop back and indentation.
        /// </summary>
        void Unindent();

        /// <summary>
        /// Output trace message.
        /// </summary>
        /// <param name="message">Message to output.</param>
        void Trace(string message);

        /// <summary>
        /// Output informational message.
        /// </summary>
        /// <param name="message">Message to output.</param>
        void Information(string message);

        /// <summary>
        /// Output an error message.
        /// </summary>
        /// <param name="message">Message to output.</param>
        void Error(string message);

        /// <summary>
        /// Output a warning message.
        /// </summary>
        /// <param name="message">Message to output.</param>
        void Warning(string message);
    }
}
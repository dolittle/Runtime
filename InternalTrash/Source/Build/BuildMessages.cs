// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Build
{
    /// <summary>
    /// Represents an implementation of <see cref="IBuildMessages"/>.
    /// </summary>
    [Singleton]
    public class BuildMessages : IBuildMessages
    {
        const int IndentSize = 2;
        int _indentLevel = 0;

        /// <inheritdoc/>
        public void Indent()
        {
            _indentLevel++;
        }

        /// <inheritdoc/>
        public void Unindent()
        {
            _indentLevel--;
            if (_indentLevel < 0) _indentLevel = 0;
        }

        /// <inheritdoc/>
        public void Trace(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(HandleIndentationFor(message));
            Console.ResetColor();
        }

        /// <inheritdoc/>
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(HandleIndentationFor(message));
            Console.ResetColor();
        }

        /// <inheritdoc/>
        public void Information(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(HandleIndentationFor(message));
            Console.ResetColor();
        }

        /// <inheritdoc/>
        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(HandleIndentationFor(message));
            Console.ResetColor();
        }

        string HandleIndentationFor(string message)
        {
            return $"{new string(' ', IndentSize * _indentLevel)}{message}";
        }
    }
}
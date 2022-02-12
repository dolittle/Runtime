// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary;

/// <summary>
/// Extensions for <see cref="Exception"/>.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Gets the inner most <see cref="Exception"/> in the provided <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to traverse.</param>
    /// <returns>The inner most <see cref="Exception"/>.</returns>
    public static Exception GetInnerMostException(this Exception exception)
    {
        while (exception.InnerException != default)
        {
            exception = exception.InnerException;
        }

        return exception;
    }
}

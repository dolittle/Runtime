// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary;

/// <summary>
/// Extensions for Task.
/// </summary>
public static class TaskExtension
{
    /// <summary>
    /// Tries to get the first innermost <see cref="Exception" /> in a list of <see cref="Task" />.
    /// </summary>
    /// <param name="tasks"><see cref="IEnumerable{T}"/> of <see cref="Task" />.</param>
    /// <param name="exception">The first innermost <see cref="Exception"/>.</param>
    /// <returns>true if any exceptions, false if not.</returns>
    public static bool TryGetFirstInnerMostException(this IEnumerable<Task> tasks, out Exception exception)
    {
        exception = tasks.FirstOrDefault(_ => _.Exception != default)?.Exception;
        if (exception != default)
        {
            while (exception.InnerException != null) exception = exception.InnerException;
        }

        return exception != default;
    }
}
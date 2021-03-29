// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Projections.Store.Services
{
    /// <summary>
    /// Extension methods for <see cref="Exception" />.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Converts the <see cref="Exception" /> to a <see cref="Failure" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to convert from.</param>
        /// <returns>The converted <see cref="Failure" />.</returns>
        public static Failure ToFailure(this Exception exception)
            => exception switch
            {
                FailedToGetProjectionDefinition e => new Failure(ProjectionsFailures.FailedToGetProjectionDefinition, e.Message),
                _ => new Failure(FailureId.Other, $"Error message: {exception.Message}\nStack Trace: {exception.StackTrace}")
            };
    }
}

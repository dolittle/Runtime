// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Tasks
{
    /// <summary>
    /// Defines an identifier for <see cref="Task">Tasks</see>.
    /// </summary>
    public class TaskId
    {
        /// <summary>
        /// Gets or sets the actual value.
        /// </summary>
        public Guid Value { get; set; }

        /// <summary>
        /// Implicitly convert from <see cref="TaskId"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="taskId"><see cref="TaskId"/> to convert from.</param>
        /// <returns>The <see cref="Guid"/> from the <see cref="TaskId"/>.</returns>
        public static implicit operator Guid(TaskId taskId) => taskId.Value;

        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="TaskId"/>.
        /// </summary>
        /// <param name="taskId"><see cref="Guid"/> to convert from.</param>
        /// <returns>The <see cref="TaskId"/> created from the <see cref="Guid"/>.</returns>
        public static implicit operator TaskId(Guid taskId) => new TaskId { Value = taskId };

        /// <summary>
        /// Create a new <see cref="TaskId"/>.
        /// </summary>
        /// <returns>New <see cref="TaskId"/>.</returns>
        public static TaskId New()
        {
            return Guid.NewGuid();
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Tasks
{
    /// <summary>
    /// Defines a task that can run and can potentially be paused, resumed and persisted.
    /// </summary>
    public abstract class Task
    {
        /// <summary>
        /// The event that gets called when a state change has occured on the <see cref="Task"/>.
        /// </summary>
        public event TaskStateChange StateChange;

        /// <summary>
        /// Gets or sets the current operation the task is on.
        /// </summary>
        public int CurrentOperation { get; set; }

        /// <summary>
        /// Gets or sets <see cref="TaskId">Identifier</see> of the task.
        /// </summary>
        public TaskId Id { get; set; }

        /// <summary>
        /// Gets get the operations for the task.
        /// </summary>
#pragma warning disable CA1819
        public abstract TaskOperation[] Operations { get; }
#pragma warning restore CA1819

        /// <summary>
        /// Gets a value indicating whether or not operations can run asynchronously, default is true.
        /// </summary>
        /// <remarks>
        /// Override this to change the default behavior of it running everything asynchronously.
        /// </remarks>
        public virtual bool CanRunOperationsAsynchronously => true;

        /// <summary>
        /// Gets a value indicating whether the task is done or not.
        /// </summary>
        public bool IsDone => CurrentOperation >= Operations.Length;

        /// <summary>
        /// Gets called when the task is about to begin.
        /// </summary>
        public virtual void Begin()
        {
        }

        /// <summary>
        /// Gets called when the task is ended, meaning when all the operations are done.
        /// </summary>
        public virtual void End()
        {
        }

        /// <summary>
        /// Progress the state, causes a <see cref="TaskStateChange"/> event.
        /// </summary>
        public void Progress()
        {
            StateChange?.Invoke(this);
        }
    }
}

// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents a reason for why a <see cref="IRule"/> is broken.
    /// </summary>
    public class Reason
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reason"/> class.
        /// </summary>
        /// <remarks>
        /// Private constructor - disables instantiation without going through <see cref="Create"/>.
        /// </remarks>
        private Reason()
        {
        }

        /// <summary>
        /// Gets the identifier for the <see cref="Reason"/>.
        /// </summary>
        public ReasonId Id { get; private set; }

        /// <summary>
        /// Gets the title of the <see cref="BrokenRule"/>.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the Description of the <see cref="BrokenRule"/>.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Reason"/> from a given unique identifier.
        /// </summary>
        /// <param name="id"><see cref="ReasonId">Unique identifier</see> of the reason.</param>
        /// <param name="title">Title to use for the <see cref="Reason"/>.</param>
        /// <param name="description">Optional detailed description to use in the <see cref="Reason"/>.</param>
        /// <returns>A <see cref="Reason"/> instance.</returns>
        public static Reason Create(ReasonId id, string title, string description = "")
        {
            return new Reason
            {
                Id = id,
                Title = title,
                Description = description
            };
        }

        /// <summary>
        /// Create a <see cref="Cause"/> without any arguments.
        /// </summary>
        /// <returns>A new <see cref="Cause"/>.</returns>
        public Cause NoArgs()
        {
            return WithArgs(new { });
        }

        /// <summary>
        /// Create a <see cref="Cause"/> with given arguments.
        /// </summary>
        /// <param name="args">Arguments to give.</param>
        /// <returns><see cref="Cause"/>.</returns>
        /// <remarks>
        /// The arguments will be used in rendering of <see cref="Title"/> and <see cref="Description"/> strings.
        /// </remarks>
        public Cause WithArgs(object args)
        {
            return new Cause(this, args);
        }
    }
}

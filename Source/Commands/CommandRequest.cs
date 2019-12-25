// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Execution;

namespace Dolittle.Runtime.Commands
{
    /// <summary>
    /// Represents a request for executing a command.
    /// </summary>
    public class CommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRequest"/> class.
        /// </summary>
        /// <param name="correlationId"><see cref="CorrelationId"/> for the request.</param>
        /// <param name="artifactId"><see cref="ArtifactId">Identifier</see> of the command.</param>
        /// <param name="generation"><see cref="ArtifactGeneration">Generation</see> of the command.</param>
        /// <param name="content">Content of the command.</param>
        public CommandRequest(CorrelationId correlationId, ArtifactId artifactId, ArtifactGeneration generation, IDictionary<string, object> content)
        {
            CorrelationId = correlationId;
            Type = new Artifact(artifactId, generation);
            Content = content;
        }

        /// <summary>
        /// Gets the <see cref="CorrelationId"/> representing the request.
        /// </summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="Artifact"/> representing the type of the Command.
        /// </summary>
        public Artifact Type { get; }

        /// <summary>
        /// Gets the content of the command.
        /// </summary>
        public IDictionary<string, object> Content { get; }
    }
}
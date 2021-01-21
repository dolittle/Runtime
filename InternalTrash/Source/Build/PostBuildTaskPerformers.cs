// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Types;

namespace Dolittle.Build
{
    /// <summary>
    /// Represents an implementation of <see cref="IPostBuildTaskPerformers"/>.
    /// </summary>
    public class PostBuildTaskPerformers : IPostBuildTaskPerformers
    {
        readonly IInstancesOf<ICanPerformPostBuildTask> _runners;
        readonly IBuildMessages _buildMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostBuildTaskPerformers"/> class.
        /// </summary>
        /// <param name="runners">Runners to run.</param>
        /// <param name="buildMessages"><see cref="IBuildMessages"/> for build messages.</param>
        public PostBuildTaskPerformers(
            IInstancesOf<ICanPerformPostBuildTask> runners,
            IBuildMessages buildMessages)
        {
            _runners = runners;
            _buildMessages = buildMessages;
        }

        /// <inheritdoc/>
        public void Perform()
        {
            _buildMessages.Information("Perform post build tasks");
            _buildMessages.Indent();
            _runners.ForEach(_ =>
            {
                _buildMessages.Trace($"{_.Message} (Post Task: '{_.GetType().AssemblyQualifiedName}')");
                _buildMessages.Indent();
                _.Perform();
                _buildMessages.Unindent();
            });
            _buildMessages.Unindent();
        }
    }
}
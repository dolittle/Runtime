// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanConfigureLogAppenders"/>.
    /// </summary>
    public class LogAppendersConfiguration : ICanConfigureLogAppenders
    {
        /// <inheritdoc/>
        public void Configure(ILogAppenders logAppenders)
        {
            logAppenders.Add(new ManagementLogAppender());
        }
    }
}